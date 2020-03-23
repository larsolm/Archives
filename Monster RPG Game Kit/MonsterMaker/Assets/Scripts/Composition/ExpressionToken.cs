namespace PiRhoSoft.MonsterMaker
{
	public enum ExpressionTokenType
	{
		Integer,
		Number,
		Identifier,
		Command,
		Operator,
		StartGroup,
		EndGroup,
		Separator
	}

	public class ExpressionToken
	{
		public int Location;
		public ExpressionTokenType Type;
		public string Text;
		public int Integer;
		public float Number;
	}
}
