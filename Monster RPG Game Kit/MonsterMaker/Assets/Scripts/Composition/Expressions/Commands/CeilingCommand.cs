using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class CeilingCommand : CommandExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			if (Parameters.Count == 1)
			{
				var result = Parameters[0].Evaluate(context);

				switch (result.Type)
				{
					case ExpressionResultType.Boolean: return new ExpressionResult(true);
					case ExpressionResultType.Number: return new ExpressionResult(Mathf.CeilToInt(result.Number));
				}

				return result;
			}
			else
			{
				throw new ExpressionEvaluationException(this, "Ceiling can only accept exactly 1 parameter");
			}
		}
	}
}
