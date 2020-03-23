using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class Ability : ScriptableObject
	{
		public Ecosystem Ecosystem;

		[Tooltip("The instructions to run when this ability is used in the world.")] public Instruction WorldInstructions;
		[Tooltip("The instructions to run when this ability is used in battle.")] public Instruction BattleInstructions;
		[Tooltip("The traits that belong to this ability.")] public VariableStore Traits = new VariableStore();
	}
}
