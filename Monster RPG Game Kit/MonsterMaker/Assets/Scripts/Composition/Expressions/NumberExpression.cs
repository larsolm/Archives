using System.Text;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class NumberExpression : Expression
	{
		public NumberExpression(float value)
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

		float _value;
	}
}
