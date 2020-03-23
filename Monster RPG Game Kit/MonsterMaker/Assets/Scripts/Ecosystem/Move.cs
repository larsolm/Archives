using System;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class Move
	{
		public Ability Ability;
		public VariableStore Traits = new VariableStore();

		public Move(Ability ability)
		{
			Ability = ability;
		}
	}
}
