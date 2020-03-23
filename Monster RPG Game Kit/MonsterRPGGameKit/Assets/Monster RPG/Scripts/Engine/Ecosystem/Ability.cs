using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public enum AbilityUseLocation
	{
		World,
		Battle
	}

	[Serializable]
	public class AbilityVariableSource : ObjectVariableSource<Ability> { }

	[HelpURL(MonsterRpg.DocumentationUrl + "ability")]
	[CreateAssetMenu(menuName = "PiRho Soft/Ability", fileName = nameof(Ability), order = 202)]
	public class Ability : Resource, IVariableStore, IReloadable
	{
		private const string _missingEcosystemWarning = "(EAME) The Ability '{0}' has not been assigned an Ecosystem";

		private static PropertyMap _propertyMap;

		[Tooltip("The ecosystem this ability is a part of")] [ReloadOnChange] public Ecosystem Ecosystem;
		[Tooltip("The display name for this ability")] public string Name;
		[Tooltip("The traits this ability has according to the schema for its type")] public VariableList Traits = new VariableList();
		[Tooltip("The Expression to run to check if the Ability can be used in the World")] public Expression UseInWorldCondition = new Expression();
		[Tooltip("The Instructions to run when the Ability is used in the World")] public InstructionCaller UseInWorldInstruction = new InstructionCaller();
		[Tooltip("The Expression to run to check if the Ability can be used in Battle")] public Expression UseInBattleCondition = new Expression();
		[Tooltip("The Instructions to run when the Ability is used in Battle")] public InstructionCaller UseInBattleInstruction = new InstructionCaller();

		private MappedVariableStore _traitStore = new MappedVariableStore();

		public void OnEnable()
		{
			if (Ecosystem != null)
			{
				SetupTraits();
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
		}

		#region Usage

		public virtual bool IsUsableInWorld(IVariableStore variables)
		{
			return UseInWorldCondition.IsValid ? UseInWorldCondition.Execute(this, variables, VariableType.Boolean).Boolean : false;
		}

		public virtual IEnumerator UseInWorld(InstructionStore variables)
		{
			yield return UseInWorldInstruction.Execute(variables.Context, variables.This);
		}

		public virtual bool IsUsableInBattle(IVariableStore variables)
		{
			return UseInBattleCondition.IsValid ? UseInBattleCondition.Execute(this, variables, VariableType.Boolean).Boolean : false;
		}

		public virtual IEnumerator UseInBattle(InstructionStore variables)
		{
			yield return UseInBattleInstruction.Execute(variables.Context, variables.This);
		}

		#endregion

		#region Moves

		public virtual Move CreateMove(Creature creature)
		{
			var move = CreateInstance<Move>();
			move.Ability = this;
			move.Name = Name;
			move.Setup(creature);
			return move;
		}

		#endregion

		#region Traits

		private void SetupTraits()
		{
			var propertyMap = GetPropertyMap();
			var variableMap = Ecosystem.GetAbilityMap(propertyMap);

			_traitStore.Setup(variableMap, propertyMap.CreateList(this), Traits);
			Traits.Setup(Ecosystem.AbilitySchema, this);
		}

		protected static VariableValue GetName(Ability ability) => VariableValue.Create(ability.Name);

		protected void AddPropertiesToMap<AbilityType>(PropertyMap<AbilityType> map) where AbilityType : Ability
		{
			map.Add(nameof(Name), GetName, null);
		}

		protected virtual PropertyMap GetPropertyMap()
		{
			if (_propertyMap == null)
			{
				var map = new PropertyMap<Ability>();
				AddPropertiesToMap(map);

				_propertyMap = map;
			}

			return _propertyMap;
		}

		#endregion

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => _traitStore.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => _traitStore.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => _traitStore.GetVariableNames();

		#endregion
	}
}
