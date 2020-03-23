namespace PiRhoSoft.MonsterMaker
{
	public class AssignOperator : InfixOperatorExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			var result = Right.Evaluate(context);
			return Assign(context, result);
		}

		protected ExpressionResult Assign(InstructionContext context, ExpressionResult result)
		{
			var left = Left as IdentifierExpression;

			if (left == null)
				throw new ExpressionEvaluationException(this, "only identifiers can be assigned");

			var store = context.GetStore(left.Variable);

			if (store != null)
			{
				switch (result.Type)
				{
					case ExpressionResultType.Boolean:
						store.ChangeOrAdd(left.Variable.Name, result.Boolean);
						break;
					case ExpressionResultType.Integer:
						store.ChangeOrAdd(left.Variable.Name, result.Integer);
						break;
					case ExpressionResultType.Number:
						store.ChangeOrAdd(left.Variable.Name, result.Number);
						break;
				}
			}
			else
			{
				throw new ExpressionEvaluationException(this, string.Format("unable to assign to variable {0}", left.Variable.Name));
			}

			return result;
		}
	}
}
