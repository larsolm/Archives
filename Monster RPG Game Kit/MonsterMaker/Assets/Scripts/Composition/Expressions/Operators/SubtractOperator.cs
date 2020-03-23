namespace PiRhoSoft.MonsterMaker
{
	public class SubtractOperator : InfixOperatorExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			var left = Left.Evaluate(context);
			var right = Right.Evaluate(context);

			if (left.Type == ExpressionResultType.Number || right.Type == ExpressionResultType.Number)
				return new ExpressionResult(left.Number - right.Number);

			return new ExpressionResult(left.Integer - right.Integer);
		}
	}
}
