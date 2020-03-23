namespace PiRhoSoft.MonsterMaker
{
	public class ModuloAssignOperator : AssignOperator
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			var left = Left.Evaluate(context);
			var right = Right.Evaluate(context);

			if (left.Type == ExpressionResultType.Number || right.Type == ExpressionResultType.Number || right.Integer == 0)
				return Assign(context, new ExpressionResult(left.Number % right.Number));

			return Assign(context, new ExpressionResult(left.Integer % right.Integer));
		}
	}
}
