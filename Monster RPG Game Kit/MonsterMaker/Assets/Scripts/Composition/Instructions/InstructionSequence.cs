using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class InstructionSequence : Instruction
	{
		public List<Instruction> Instructions = new List<Instruction>();

		public override void Begin(InstructionContext context)
		{
			context.RunInSequence(Instructions, context.Parent);
		}
	}
}
