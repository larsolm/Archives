using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public interface ITrainer : IVariableStore
	{
		string Name { get; }
		Roster Roster { get; }
	}

	[ExecuteInEditMode] // for OnEnable initialization of Traits, Roster, and Inventory
	[DisallowMultipleComponent]
	[HelpURL(MonsterRpg.DocumentationUrl + "trainer")]
	[AddComponentMenu("PiRho Soft/Ecosystem/Trainer")]
	public class Trainer : MonoBehaviour, ITrainer, IVariableReset, IReloadable
	{
		private const string _missingEcosystemWarning = "(ETME) The Trainer '{0}' has not been assigned an Ecosystem";

		private static PropertyMap _propertyMap;

		[Tooltip("The ecosystem this trainer is a part of")] [ReloadOnChange] public Ecosystem Ecosystem;
		[Tooltip("The traits this trainer has")] public VariableList Traits = new VariableList();
		[Tooltip("The items this trainer has")] public Inventory Inventory = new Inventory();
		[Tooltip("The creatures this trainer has")] [SerializeField] public Roster _roster = new Roster();

		private MappedVariableStore _traitStore = new MappedVariableStore();

		public void OnEnable()
		{
			if (Ecosystem == null && ApplicationHelper.IsPlaying)
				Debug.LogWarningFormat(this, _missingEcosystemWarning, gameObject.name);

			SetupTraits();
			Inventory.Setup(this);
			_roster.Setup();

			if (ApplicationHelper.IsPlaying)
				_roster.CreateCreatures(this);
		}

		public void OnDisable()
		{
			if (ApplicationHelper.IsPlaying)
				_roster.DestroyCreatures();
		}

		#region Traits

		private void SetupTraits()
		{
			var propertyMap = GetPropertyMap();

			if (Ecosystem != null)
			{
				var variableMap = Ecosystem.GetTrainerMap(_propertyMap);
				_traitStore.Setup(variableMap, _propertyMap.CreateList(this), Traits);
				Traits.Setup(Ecosystem.TrainerSchema, this);
			}
			else
			{
				var variableMap = new VariableMap(0).Add(_propertyMap);
				_traitStore.Setup(variableMap, _propertyMap.CreateList(this));
			}
		}

		protected static VariableValue GetName(Trainer trainer) => VariableValue.Create(trainer.Name);
		protected static VariableValue GetRoster(Trainer trainer) => VariableValue.Create(trainer.Roster);
		protected static VariableValue GetInventory(Trainer trainer) => VariableValue.Create(trainer.Inventory);

		protected void AddPropertiesToMap<TrainerType>(PropertyMap<TrainerType> map) where TrainerType : Trainer
		{
			map.Add(nameof(Name), GetName, null)
				.Add(nameof(Roster), GetRoster, null)
				.Add(nameof(Inventory), GetInventory, null);
		}

		protected virtual PropertyMap GetPropertyMap()
		{
			if (_propertyMap == null)
			{
				var map = new PropertyMap<Trainer>();
				AddPropertiesToMap(map);
				_propertyMap = map;
			}

			return _propertyMap;
		}

		#endregion

		#region ITrainer Implementation
		
		public Roster Roster => _roster;

		public string Name
		{
			get
			{
				var player = GetComponent<Player>();
				if (player != null)
					return player.Name;

				var npc = GetComponent<Npc>();
				if (npc != null)
					return npc.Name;

				return "";
			}
		}

		#endregion

		#region IVariableReset Implementation

		public void ResetAvailability(string availability)
		{
			Traits.ResetAvailability(availability);
			Roster.ResetAvailability(availability);
		}

		public void ResetVariables(IList<string> traits)
		{
			Traits.ResetVariables(traits);
			Roster.ResetVariables(traits);
		}

		#endregion

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => _traitStore.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => _traitStore.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => _traitStore.GetVariableNames();

		#endregion
	}
}
