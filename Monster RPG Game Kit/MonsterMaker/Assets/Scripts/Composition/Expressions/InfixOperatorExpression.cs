using System.Text;

namespace PiRhoSoft.MonsterMaker
{
	public struct OperatorPrecedence
	{
		public static OperatorPrecedence Assignment = RightAssociative(2);
		public static OperatorPrecedence Or = LeftAssociative(4);
		public static OperatorPrecedence And = LeftAssociative(6);
		public static OperatorPrecedence Equality = LeftAssociative(8);
		public static OperatorPrecedence Comparison = LeftAssociative(10);
		public static OperatorPrecedence Addition = LeftAssociative(12);
		public static OperatorPrecedence Multiplication = LeftAssociative(14);
		public static OperatorPrecedence Exponentiation = RightAssociative(16);

		public static OperatorPrecedence LeftAssociative(int value)
		{
			return new OperatorPrecedence { Value = value, AssociativeValue = value };
		}

		public static OperatorPrecedence RightAssociative(int value)
		{
			return new OperatorPrecedence { Value = value, AssociativeValue = value - 1 };
		}

		public int Value { get; private set; }
		public int AssociativeValue { get; private set; }
	}

	public abstract class InfixOperatorExpression : Expression
	{
		public Expression Left;
		public string Symbol;
		public Expression Right;

		public override void ToString(StringBuilder builder)
		{
			Left.ToString(builder);
			builder.Append(' ');
			builder.Append(Symbol);
			builder.Append(' ');
			Right.ToString(builder);
		}
	}
}
