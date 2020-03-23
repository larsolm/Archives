namespace PiRhoSoft.MonsterMaker
{
	public class TruncateCommand : CommandExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			if (Parameters.Count == 1)
			{
				var result = Parameters[0].Evaluate(context);

				switch (result.Type)
				{
					case ExpressionResultType.Boolean: return new ExpressionResult(false);
					case ExpressionResultType.Number: return new ExpressionResult(result.Integer);
				}

				return result;
			}
			else
			{
				throw new ExpressionEvaluationException(this, "Truncate can only accept exactly 1 parameter");
			}
		}
	}
}
