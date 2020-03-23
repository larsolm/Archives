using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class ExpressionInstruction : Instruction
	{
		public List<ExpressionStatement> Statements = new List<ExpressionStatement>();

		public override void Begin(InstructionContext context)
		{
			foreach (var statement in Statements)
				statement.Execute(context);
		}
	}
}
