using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public struct MountPoint
	{
		public float X;
		public float Y;
		public float Rotation;
	}

	[Serializable] public class MountPointDictionary : SerializedDictionary<string, MountPoint> { }

	[HelpURL(MonsterRpg.DocumentationUrl + "species")]
	[CreateAssetMenu(menuName = "PiRho Soft/Species", fileName = nameof(Species), order = 201)]
	public class Species : Resource, IVariableStore, IReloadable
	{
		private const string _missingEcosystemWarning = "(ESME) The Species '{0}' has not been assigned an Ecosystem";

		private static PropertyMap _propertyMap;

		[Tooltip("The ecosystem this species is a part of")] [ReloadOnChange] public Ecosystem Ecosystem;
		[Tooltip("The name of this species")] public string Name;
		[Tooltip("The icon for this species")] public Sprite Icon;
		[Tooltip("The animations for this species")] public AnimatorOverrideController Animations;
		[Tooltip("The traits this species has")] public VariableList Traits = new VariableList();
		[Tooltip("The skills available to Creatures of this Species")] [ListDisplay(ItemDisplay = ListItemDisplayType.Inline, EmptyText = "The Species has no Skills")] public SkillList Skills = new SkillList();
		[Tooltip("The mount point locations on the Species")] [DictionaryDisplay(ItemDisplay = ListItemDisplayType.Inline, EmptyText = "The Species has no mount points")] public MountPointDictionary MountPoints = new MountPointDictionary();

		private MappedVariableStore _traitStore = new MappedVariableStore();
		private List<List<int>> _traitTriggers = new List<List<int>>();
		private List<List<int>> _skillTriggers = new List<List<int>>();

		public void OnEnable()
		{
			if (Ecosystem != null)
			{
				SetupTraits();
				SetupTriggers();
			}
			else
			{
				if (ApplicationHelper.IsPlaying)
					Debug.LogWarningFormat(this, _missingEcosystemWarning, name);

				Traits.Clear();
			}
		}

		public void OnDisable()
		{
			_traitTriggers.Clear();
			_skillTriggers.Clear();
		}

		#region Creatures

		public virtual Creature CreateCreature(ITrainer trainer)
		{
			var creature = CreateInstance<Creature>();
			creature.Species = this;
			creature.name = name;
			creature.Name = Name;
			creature.Setup(trainer);
			return creature;
		}

		#endregion

		#region Traits

		private void SetupTraits()
		{
			var propertyMap = GetPropertyMap();
			var variableMap = Ecosystem.GetSpeciesMap(propertyMap);

			Traits.Setup(Ecosystem.SpeciesSchema, this);
			_traitStore.Setup(variableMap, propertyMap.CreateList(this), Traits);
		}

		protected static VariableValue GetName(Species species) => VariableValue.Create(species.Name);
		protected static VariableValue GetIcon(Species species) => VariableValue.Create(species.Icon);

		protected void AddPropertiesToMap<SpeciesType>(PropertyMap<SpeciesType> map) where SpeciesType : Species
		{
			map.Add(nameof(Name), GetName, null)
				.Add(nameof(Icon), GetIcon, null);
		}

		protected virtual PropertyMap GetPropertyMap()
		{
			if (_propertyMap == null)
			{
				var map = new PropertyMap<Species>();
				AddPropertiesToMap(map);

				_propertyMap = map;
			}

			return _propertyMap;
		}

		#endregion

		#region Triggers

		public List<int> GetTriggeredTraits(int index)
		{
			return _traitTriggers[index];
		}

		public List<int> GetTriggeredSkills(int index)
		{
			return _skillTriggers[index];
		}

		private void SetupTriggers()
		{
			for (var i = 0; i < Ecosystem.CreatureSchema.Count; i++)
			{
				_traitTriggers.Add(new List<int>());
				_skillTriggers.Add(new List<int>());
			}

			var inputs = new List<VariableDefinition>();

			for (var i = 0; i < Ecosystem.CreatureSchema.Count; i++)
			{
				Ecosystem.CreatureSchema[i].Initializer?.GetInputs(inputs, null);

				foreach (var input in inputs)
					AddTrigger(_traitTriggers, input.Name, i);

				inputs.Clear();
			}

			for (var i = 0; i < Skills.Count; i++)
			{
				Skills[i].Condition.GetInputs(inputs, null);

				foreach (var input in inputs)
					AddTrigger(_skillTriggers, input.Name, i);

				inputs.Clear();
			}
		}

		private void AddTrigger(List<List<int>> triggers, string name, int index)
		{
			var triggerIndex = Ecosystem.CreatureSchema.GetIndex(name);
			var trigger = triggers[triggerIndex];

			if (!trigger.Contains(index))
				trigger.Add(index);
		}

		#endregion

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => _traitStore.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => _traitStore.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => _traitStore.GetVariableNames();

		#endregion
	}
}
