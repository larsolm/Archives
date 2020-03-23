using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class RoundCommand : CommandExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			if (Parameters.Count == 1)
			{
				var result = Parameters[0].Evaluate(context);

				switch (result.Type)
				{
					case ExpressionResultType.Boolean: return new ExpressionResult(false);
					case ExpressionResultType.Number: return new ExpressionResult(Mathf.RoundToInt(result.Number));
				}

				return result;
			}
			else
			{
				throw new ExpressionEvaluationException(this, "Round can only accept exactly 1 parameter");
			}
		}
	}
}
