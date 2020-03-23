using System;
using UnityObject = UnityEngine.Object;

namespace PiRhoSoft.MonsterMaker
{
	[VariableSource("Battle")]
	public enum BattleVariableSource
	{
		Battle,
		Traits,
		Player
	}

	[VariableSource("Battle/Trainer/{0}", new string[] { "Current", "1", "2" })]
	public enum BattleTrainerVariableSource
	{
		BattleTraits,
		TrainerTraits
	}

	[VariableSource("Battle/Trainer/{0}/Creature/{1}", new string[] { "Current", "1", "2" }, new string[] { "Active", "1", "2", "3", "4", "5", "6" })]
	public enum BattleCreatureVariableSource
	{
		BattleTraits,
		CreatureTraits,
		SpeciesTraits
	}

	public class BattleInstructionContext : WorldInstructionContext
	{
		public BattleInstructionContext(UnityObject owner, string name, VariableStore variables) : base(owner, name, variables)
		{
		}

		protected override VariableStore GetCustomStore(VariableReference variable)
		{
			var sections = variable.CustomSource.Split('/');

			if (variable.CustomSource == "Battle")
			{
				return GetVariableStore((BattleVariableSource)variable.CustomIndex);
			}
			else if (Enum.IsDefined(typeof(BattleVariableSource), variable.CustomSource))
			{
				var source = (BattleVariableSource)Enum.Parse(typeof(BattleVariableSource), variable.CustomSource);
				return GetVariableStore(source);
			}

			// Battle - read only store on Battle asset
			// BattleData - BattleData store
			// Player - store on Player instance
			// Trainer[#] - store on Trainer component
			// Creature[#] - store on Creature
			// Species[#] - store on Species
			// TrainerData[#] - store on TrainerData
			// CreatureData[#] - store on CreatureData

			return base.GetCustomStore(variable);
		}

		private VariableStore GetVariableStore(BattleVariableSource source)
		{
			return null;
		}
	}
}
