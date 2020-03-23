namespace PiRhoSoft.MonsterMaker
{
	public class OrAssignOperator : AssignOperator
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			var left = Left.Evaluate(context);
			var right = Right.Evaluate(context);

			return Assign(context, new ExpressionResult(left.Boolean || right.Boolean));
		}
	}
}
