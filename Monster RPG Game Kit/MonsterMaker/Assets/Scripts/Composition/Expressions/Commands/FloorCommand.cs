using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class FloorCommand : CommandExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			if (Parameters.Count == 1)
			{
				var result = Parameters[0].Evaluate(context);

				switch (result.Type)
				{
					case ExpressionResultType.Boolean: return new ExpressionResult(false);
					case ExpressionResultType.Number: return new ExpressionResult(Mathf.FloorToInt(result.Number));
				}

				return result;
			}
			else
			{
				throw new ExpressionEvaluationException(this, "Floor can only accept exactly 1 parameter");
			}
		}
	}
}
