using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class CreatureSaveData
	{
		public string SpeciesPath = "";
		public string Name = "";
		public VariableList Traits = new VariableList();
		public SkillsDictionary LearnedSkills = new SkillsDictionary();
		public List<MoveSaveData> Moves = new List<MoveSaveData>();
	}

	[Serializable]
	public class CreatureReference : IVariableStore
	{
		private const string _missingSpeciesWarning = "(ECRMS) A CreatureReference on '{0}' has not been assigned a Species";

		public Creature Creature;
		public Species Species;
		public InstructionCaller Generator;

		public void Setup()
		{
			if (Creature)
				Creature.Setup(null);
		}

		public Creature CreateCreature(ITrainer trainer)
		{
			if (Creature && Creature.Species)
			{
				return Creature.Clone(trainer);
			}
			else if (Species)
			{
				var creature = Species.CreateCreature(trainer);

				if (Generator != null)
					InstructionManager.Instance.RunInstruction(Generator, null, Creature);

				return creature;
			}
			else
			{
				Debug.LogWarningFormat(_missingSpeciesWarning, (trainer as Object)?.name);
				return null;
			}
		}

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => Creature && Creature.Species ? Creature.GetVariable(name) : Species ? Species.GetVariable(name) : VariableValue.Empty;
		public SetVariableResult SetVariable(string name, VariableValue value) => Creature && Creature.Species ? Creature.SetVariable(name, value) : Species ? Species.SetVariable(name, value) : SetVariableResult.NotFound;
		public IEnumerable<string> GetVariableNames() => Creature && Creature.Species ? Creature.GetVariableNames() : Species ? Species.GetVariableNames() : Enumerable.Empty<string>();

		#endregion
	}

	[HelpURL(MonsterRpg.DocumentationUrl + "creature")]
	public class Creature : ScriptableObject, IVariableReset, IVariableStore, IVariableListener
	{
		private const string _missingAbilityWarning = "(ECMA) The Ability '{0}' could not be found";
		private const string _deletedSpeciesWarning = "(ECDS) The Species for Creature '{0}' has been deleted";

		private static PropertyMap _propertyMap;

		[Tooltip("The Species this creature is an instance of")]
		[DisableInInspector]
		public Species Species;

		[Tooltip("The name of this creature")]
		public string Name;

		[Tooltip("The traits this creature has")]
		public VariableList Traits = new VariableList();

		[Tooltip("The moves this creature has learned")]
		public MoveList Moves = new MoveList();

		private MappedVariableStore _traitStore = new MappedVariableStore();
		private SkillsDictionary _learnedSkills = new SkillsDictionary();
		private List<int> _pendingTraits = new List<int>();
		private List<int> _pendingSkills = new List<int>();

		public ITrainer Trainer { get; private set; }

		public void Setup(ITrainer trainer)
		{
			Trainer = trainer;

			if (Species == null)
			{
				Debug.LogWarningFormat(this, _deletedSpeciesWarning, Name);
			}
			else if (Species.Ecosystem != null) // Species will log the error if its Ecosystem is null
			{
				SetupTraits();
				Moves.Setup(this);
			}
		}

		public void Teardown()
		{
			foreach (var move in Moves)
				Destroy(move);
		}

		public Creature Clone(ITrainer trainer)
		{
			var creature = Species.CreateCreature(trainer);

			creature.Name = Name;
			creature.Traits.LoadFrom(Traits, null);

			foreach (var move in Moves)
			{
				var m = move.Clone(this);
				creature.Moves.Add(m);
			}

			foreach (var skill in _learnedSkills)
				creature._learnedSkills.Add(skill.Key, skill.Value);

			return creature;
		}

		#region Traits

		public List<int> TakePendingTraits()
		{
			if (_pendingTraits.Count > 0)
			{
				var traits = _pendingTraits;
				_pendingTraits = new List<int>();
				return traits;
			}
			else
			{
				return null;
			}
		}

		public void UpdatePendingTraits()
		{
			var traits = TakePendingTraits();

			while (traits != null)
			{
				foreach (var trait in traits)
					UpdateTrait(trait);

				traits = TakePendingTraits();
			}
		}

		public void UpdateTrait(int index)
		{
			Traits.Reset(index);
			TraitChanged(index);
		}

		private void SetupTraits()
		{
			var propertyMap = GetPropertyMap();
			var variableMap = Species.Ecosystem.GetCreatureMap(propertyMap);

			_traitStore.Setup(variableMap, propertyMap.CreateList(this), new VariableListener(this, Traits));
			Traits.Setup(Species.Ecosystem.CreatureSchema, this);
		}

		protected static VariableValue GetSpecies(Creature creature) => VariableValue.Create(creature.Species);
		protected static VariableValue GetTrainer(Creature creature) => VariableValue.Create(creature.Trainer);
		protected static VariableValue GetName(Creature creature) => VariableValue.Create(creature.Name);
		protected static SetVariableResult SetName(Creature creature, VariableValue value) => value.TryGetString(out creature.Name) ? SetVariableResult.Success : SetVariableResult.TypeMismatch;
		protected static VariableValue GetMoves(Creature creature) => VariableValue.Create(creature.Moves);

		protected void AddPropertiesToMap<CreatureType>(PropertyMap<CreatureType> map) where CreatureType : Creature
		{
			map.Add(nameof(Species), GetSpecies, null)
				.Add(nameof(Trainer), GetTrainer, null)
				.Add(nameof(Name), GetName, SetName)
				.Add(nameof(Moves), GetMoves, null);
		}

		protected virtual PropertyMap GetPropertyMap()
		{
			if (_propertyMap == null)
			{
				var map = new PropertyMap<Creature>();
				AddPropertiesToMap(map);

				_propertyMap = map;
			}

			return _propertyMap;
		}

		protected virtual void TraitChanged(int index)
		{
			var traits = Species.GetTriggeredTraits(index);
			var skills = Species.GetTriggeredSkills(index);

			if (traits != null)
			{
				foreach (var trait in traits)
					_pendingTraits.Add(trait);
			}

			if (skills != null)
			{
				foreach (var skill in skills)
					_pendingSkills.Add(skill);
			}
		}

		#endregion

		#region Skills

		public bool HasPendingSkill()
		{
			return _pendingSkills.Count > 0;
		}

		public Skill TakePendingSkill()
		{
			if (HasPendingSkill())
			{
				var index = _pendingSkills[0];
				_pendingSkills.RemoveAt(0);
				return Species.Skills[index];
			}
			else
			{
				return null;
			}
		}

		public List<int> TakePendingSkills()
		{
			if (HasPendingSkill())
			{
				var skills = _pendingSkills;
				_pendingSkills = new List<int>();
				return skills;
			}
			else
			{
				return null;
			}
		}

		public void TeachPendingSkills()
		{
			var skills = TakePendingSkills();

			if (skills != null)
			{
				foreach (var skill in skills)
					TeachSkill(skill);
			}
		}

		public void TeachSkill(int index)
		{
			var skill = Species.Skills[index];

			if (CanLearnSkill(skill))
				TeachSkill(skill);
		}

		public bool CanLearnSkill(Skill skill)
		{
			var learnCount = _learnedSkills.TryGetValue(skill.Name, out int count) ? count : 0;

			if (skill.LearnLimit <= 0 || learnCount < skill.LearnLimit)
				return skill.Condition.Execute(this, this, VariableType.Boolean).Boolean;
			else
				return false;
		}

		public void TeachSkill(Skill skill)
		{
			InstructionManager.Instance.RunInstruction(skill.Instruction, null, this);
			SkillLearned(skill);
		}

		public IEnumerator TeachSkill(Skill skill, InstructionContext context)
		{
			yield return skill.Instruction.Execute(null, this);
			SkillLearned(skill);
		}

		private void SkillLearned(Skill skill)
		{
			if (_learnedSkills.TryGetValue(skill.Name, out int count))
				_learnedSkills[skill.Name] = count + 1;
			else
				_learnedSkills[skill.Name] = 1;
		}

		#endregion

		#region Persistence

		public static Creature Create(CreatureSaveData data, ITrainer trainer)
		{
			var species = Resources.Load<Species>(data.SpeciesPath);

			if (species != null)
			{
				var creature = species.CreateCreature(trainer);
				creature.Load(data);
				return creature;
			}

			return null;
		}

		public static CreatureSaveData Save(Creature creature)
		{
			var data = new CreatureSaveData();
			data.SpeciesPath = creature.Species ? creature.Species.Path : "";
			creature.Save(data);
			return data;
		}

		protected virtual void Load(CreatureSaveData data)
		{
			Name = data.Name;
			_learnedSkills = data.LearnedSkills;
			Traits.LoadFrom(data.Traits, VariableDefinition.Saved);

			foreach (var moveData in data.Moves)
			{
				var move = Move.Create(this, moveData);

				if (move != null)
					Moves.Add(move);
				else
					Debug.LogWarningFormat(this, _missingAbilityWarning, Name);
			}
		}

		protected virtual void Save(CreatureSaveData data)
		{
			data.Name = Name;
			data.LearnedSkills = _learnedSkills;
			Traits.SaveTo(data.Traits, VariableDefinition.Saved);

			foreach (var move in Moves)
				data.Moves.Add(Move.Save(move));
		}

		#endregion

		#region IVariableReset Implementation

		public virtual void ResetAvailability(string availability)
		{
			Traits.ResetAvailability(availability);
			Moves.ResetAvailability(availability);
		}

		public virtual void ResetVariables(IList<string> traits)
		{
			Traits.ResetVariables(traits);
			Moves.ResetVariables(traits);
		}

		#endregion

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => _traitStore.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => _traitStore.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => _traitStore.GetVariableNames();

		#endregion

		#region IVariableListener Implementation

		public void VariableChanged(int index, VariableValue value)
		{
			TraitChanged(index);
		}

		#endregion
	}
}
