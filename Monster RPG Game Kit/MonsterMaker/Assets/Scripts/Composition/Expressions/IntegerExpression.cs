using System.Text;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class IntegerExpression : Expression
	{
		public IntegerExpression(int value)
		{
			_value = value;
		}

		public override ExpressionResult Evaluate(InstructionContext context)
		{
			return new ExpressionResult(_value);
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(_value);
		}

		int _value;
	}
}
