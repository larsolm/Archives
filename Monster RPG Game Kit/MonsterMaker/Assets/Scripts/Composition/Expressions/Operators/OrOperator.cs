﻿namespace PiRhoSoft.MonsterMaker
{
	public class OrOperator : InfixOperatorExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			var left = Left.Evaluate(context);
			var right = Right.Evaluate(context);

			return new ExpressionResult(left.Boolean || right.Boolean);
		}
	}
}
