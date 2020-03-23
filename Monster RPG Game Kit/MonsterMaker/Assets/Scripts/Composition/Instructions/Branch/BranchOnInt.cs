using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class BranchOnInt : BranchInstruction<int>
	{
		[Tooltip("The instructions to run based on the value")] public List<Instruction> Instructions = new List<Instruction>();

		protected override Instruction GetInstruction(int value)
		{
			return value >= 0 && value < Instructions.Count ? Instructions[value] : null;
		}
	}
}
