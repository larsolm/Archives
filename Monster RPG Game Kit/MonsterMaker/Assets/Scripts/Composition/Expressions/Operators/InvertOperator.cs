namespace PiRhoSoft.MonsterMaker
{
	public class InvertOperator : PrefixOperatorExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			var result = Right.Evaluate(context);
			return new ExpressionResult(!result.Boolean);
		}
	}
}
