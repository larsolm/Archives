using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class RandomCommand : CommandExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			if (Parameters.Count == 0)
			{
				return new ExpressionResult(Random.value);
			}
			else if (Parameters.Count == 1)
			{
				var max = Parameters[0].Evaluate(context);

				if (max.Type == ExpressionResultType.Integer)
					return new ExpressionResult(Random.Range(0, max.Integer));
				else
					return new ExpressionResult(Random.Range(0.0f, max.Number));
			}
			else if (Parameters.Count == 2)
			{
				var min = Parameters[0].Evaluate(context);
				var max = Parameters[1].Evaluate(context);

				if (min.Type == ExpressionResultType.Integer && max.Type == ExpressionResultType.Integer)
					return new ExpressionResult(Random.Range(min.Integer, max.Integer));
				else
					return new ExpressionResult(Random.Range(min.Number, max.Number));
			}
			else
			{
				throw new ExpressionEvaluationException(this, "Random can only accept 0, 1, or 2 parameters");
			}
		}
	}
}
