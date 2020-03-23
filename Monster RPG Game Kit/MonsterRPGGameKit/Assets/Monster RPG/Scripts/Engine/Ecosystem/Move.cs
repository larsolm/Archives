using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class MoveSaveData
	{
		public string AbilityPath = "";
		public string Name = "";
		public VariableList Traits = new VariableList();
	}

	[Serializable]
	public class MoveList : IndexedVariableStore<Move>, IVariableReset
	{
		public void Setup(Creature creature)
		{
			foreach (var move in this)
				move.Setup(creature);
		}

		public void ResetAvailability(string availability)
		{
			foreach (var move in this)
				move.ResetAvailability(availability);
		}

		public void ResetVariables(IList<string> traits)
		{
			foreach (var move in this)
				move.ResetVariables(traits);
		}
	}

	[HelpURL(MonsterRpg.DocumentationUrl + "move")]
	public class Move : ScriptableObject, IVariableReset, IVariableStore
	{
		private const string _deletedAbilityWarning = "(EMDA) The Ability for Move '{0}' has been deleted";

		private static PropertyMap _propertyMap;

		[Tooltip("The Ability this move is an instance of")] [DisableInInspector] public Ability Ability;
		[Tooltip("The creature that has this move")] [DisableInInspector] public Creature Creature;
		[Tooltip("The name of the move")] public string Name;
		[Tooltip("The traits this move has")] public VariableList Traits = new VariableList();

		private MappedVariableStore _traitStore = new MappedVariableStore();

		public void Setup(Creature creature)
		{
			if (Ability == null)
			{
				Debug.LogWarningFormat(this, _deletedAbilityWarning, Name);
			}
			else if (Ability.Ecosystem != null) // Ability will log the error if its Ecosystem is null
			{
				Creature = creature;
				SetupTraits();
			}
		}

		public Move Clone(Creature creature)
		{
			var move = Ability.CreateMove(creature);
			move.Name = Name;
			move.Traits.LoadFrom(Traits, null);
			return move;
		}

		#region Traits

		private void SetupTraits()
		{
			var propertyMap = GetPropertyMap();
			var variableMap = Ability.Ecosystem.GetMoveMap(propertyMap);

			_traitStore.Setup(variableMap, propertyMap.CreateList(this), Traits);
			Traits.Setup(Ability.Ecosystem.MoveSchema, this);
		}

		protected static VariableValue GetAbility(Move move) => VariableValue.Create(move.Ability);
		protected static VariableValue GetCreature(Move move) => VariableValue.Create(move.Creature);
		protected static VariableValue GetName(Move move) => VariableValue.Create(move.Name);
		protected static SetVariableResult SetName(Move move, VariableValue value) => value.TryGetString(out move.Name) ? SetVariableResult.Success : SetVariableResult.TypeMismatch;

		protected void AddPropertiesToMap<MoveType>(PropertyMap<MoveType> map) where MoveType : Move
		{
			map.Add(nameof(Ability), GetAbility, null)
				.Add(nameof(Creature), GetCreature, null)
				.Add(nameof(Name), GetName, SetName);
		}

		protected virtual PropertyMap GetPropertyMap()
		{
			if (_propertyMap == null)
			{
				var map = new PropertyMap<Move>();
				AddPropertiesToMap(map);

				_propertyMap = map;
			}

			return _propertyMap;
		}

		#endregion

		#region Persistence

		public static Move Create(Creature creature, MoveSaveData data)
		{
			var ability = Resources.Load<Ability>(data.AbilityPath);

			if (ability != null)
			{
				var move = ability.CreateMove(creature);
				move.Load(data);
				return move;
			}

			return null;
		}

		public static MoveSaveData Save(Move move)
		{
			var data = new MoveSaveData();
			data.AbilityPath = move.Ability ? move.Ability.Path : "";
			move.Save(data);
			return data;
		}

		protected virtual void Load(MoveSaveData data)
		{
			Name = data.Name;
			Traits.LoadFrom(data.Traits, VariableDefinition.Saved);
		}

		protected virtual void Save(MoveSaveData data)
		{
			data.Name = Name;
			Traits.SaveTo(data.Traits, VariableDefinition.Saved);
		}

		#endregion

		#region IVariableReset Implementation

		public virtual void ResetAvailability(string availability)
		{
			Traits.ResetAvailability(availability);
		}

		public virtual void ResetVariables(IList<string> traits)
		{
			Traits.ResetVariables(traits);
		}

		#endregion

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => _traitStore.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => _traitStore.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => _traitStore.GetVariableNames();

		#endregion
	}
}
