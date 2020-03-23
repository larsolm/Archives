namespace PiRhoSoft.MonsterMaker
{
	public class NegateOperator : PrefixOperatorExpression
	{
		public override ExpressionResult Evaluate(InstructionContext context)
		{
			var result = Right.Evaluate(context);
			result.Integer = -result.Integer;
			result.Number = -result.Number;
			return result;
		}
	}
}
