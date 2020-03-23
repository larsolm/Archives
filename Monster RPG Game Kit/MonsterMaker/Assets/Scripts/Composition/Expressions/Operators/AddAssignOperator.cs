namespace PiRhoSoft.MonsterMaker
{
	public class AddAssignOperator : AssignOperator
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			var left = Left.Evaluate(context);
			var right = Right.Evaluate(context);

			if (left.Type == ExpressionResultType.Number || right.Type == ExpressionResultType.Number)
				return Assign(context, new ExpressionResult(left.Number + right.Number));

			return Assign(context, new ExpressionResult(left.Integer + right.Integer));
		}
	}
}
