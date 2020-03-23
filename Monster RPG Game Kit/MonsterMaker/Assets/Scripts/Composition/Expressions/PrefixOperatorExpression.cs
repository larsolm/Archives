using System.Text;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class PrefixOperatorExpression : Expression
	{
		public string Symbol;
		public Expression Right;

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Symbol);
			Right.ToString(builder);
		}
	}
}
