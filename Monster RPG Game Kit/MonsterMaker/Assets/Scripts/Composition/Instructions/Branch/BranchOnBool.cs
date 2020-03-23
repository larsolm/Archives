using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class BranchOnBool : BranchInstruction<bool>
	{
		[Tooltip("The instruction to run if the value is true")] public Instruction OnTrue;
		[Tooltip("The instruction to run if the value is false")] public Instruction OnFalse;

		protected override Instruction GetInstruction(bool value)
		{
			return value ? OnTrue : OnFalse;
		}
	}
}
