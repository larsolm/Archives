using System;

namespace PiRhoSoft.MonsterMaker
{
	public class ExponentOperator : InfixOperatorExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			var number = Left.Evaluate(context);
			var exponent = Right.Evaluate(context);

			if (number.Type == ExpressionResultType.Number || exponent.Type == ExpressionResultType.Number)
				return new ExpressionResult((float)Math.Pow(number.Number, exponent.Number));

			return new ExpressionResult(MathHelper.IntExponent(number.Integer, exponent.Integer));
		}
	}
}
