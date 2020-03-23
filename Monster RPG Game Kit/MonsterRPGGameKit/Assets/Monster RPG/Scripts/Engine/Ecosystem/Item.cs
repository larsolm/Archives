using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public enum ItemType
	{
		Durable,
		Consumable,
		Key
	}

	public enum ItemUseLocation
	{
		World,
		Battle
	}

	[Serializable]
	public class ItemVariableSource : ObjectVariableSource<Item> { }

	[HelpURL(MonsterRpg.DocumentationUrl + "item")]
	[CreateAssetMenu(menuName = "PiRho Soft/Item", fileName = nameof(Item), order = 203)]
	public class Item : Resource, IVariableStore
	{
		[Tooltip("The name of the Item")] public string Name = "";
		[Tooltip("The description of the Item")] public string Description = "";
		[Tooltip("The type of the Item")] public ItemType Type = ItemType.Durable;
		[Tooltip("The cost of the Item when purchasing from a Shop")] public int PurchaseCost = 10000;
		[Tooltip("The value of the Item when selling")] public int SellCost = 50000;
		[Tooltip("The Expression to run to check if the Item can be used in the World")] public Expression UseInWorldCondition = new Expression();
		[Tooltip("The Instructions to run when the Item is used in the World")] public InstructionCaller UseInWorldInstruction = new InstructionCaller();
		[Tooltip("The Expression to run to check if the Item can be used in Battle")] public Expression UseInBattleCondition = new Expression();
		[Tooltip("The Instructions to run when the Item is used in Battle")] public InstructionCaller UseInBattleInstruction = new InstructionCaller();

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

		#region IVariableStore Implementation

		public virtual VariableValue GetVariable(string name)
		{
			switch (name)
			{
				case nameof(Name): return VariableValue.Create(Name);
				case nameof(Description): return VariableValue.Create(Description);
				case nameof(Type): return VariableValue.Create(Type.ToString());
				case nameof(PurchaseCost): return VariableValue.Create(PurchaseCost);
				case nameof(SellCost): return VariableValue.Create(SellCost);
				default: return VariableValue.Empty;
			}
		}

		public virtual SetVariableResult SetVariable(string name, VariableValue value)
		{
			switch (name)
			{
				case nameof(Name): return SetVariableResult.ReadOnly;
				case nameof(Description): return SetVariableResult.ReadOnly;
				case nameof(Type): return SetVariableResult.ReadOnly;
				case nameof(PurchaseCost): return SetVariableResult.ReadOnly;
				case nameof(SellCost): return SetVariableResult.ReadOnly;
				default: return SetVariableResult.NotFound;
			}
		}

		public IEnumerable<string> GetVariableNames()
		{
			return new List<string> { nameof(Name), nameof(Description), nameof(Type), nameof(Type), nameof(PurchaseCost), nameof(SellCost) };
		}

		#endregion
	}
}
