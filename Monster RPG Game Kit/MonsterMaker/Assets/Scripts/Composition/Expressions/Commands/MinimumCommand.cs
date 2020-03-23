namespace PiRhoSoft.MonsterMaker
{
	public class MinimumCommand : CommandExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			var result = new ExpressionResult(int.MaxValue);

			foreach (var parameter in Parameters)
			{
				var p = parameter.Evaluate(context);

				if ((p.Type == ExpressionResultType.Number && p.Number < result.Number) || (p.Type == ExpressionResultType.Integer && p.Integer < result.Integer))
					result = p;
			}

			return result;
		}
	}
}
