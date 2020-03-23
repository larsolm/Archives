using System;
using System.Text;

namespace PiRhoSoft.MonsterMaker
{
	public enum ExpressionResultType
	{
		Boolean,
		Integer,
		Number
	}

	public struct ExpressionResult
	{
		public ExpressionResult(bool value)
		{
			Type = ExpressionResultType.Boolean;
			Boolean = value;
			Integer = value ? 1 : 0;
			Number = value ? 1.0f : 0.0f;
		}

		public ExpressionResult(int value)
		{
			Type = ExpressionResultType.Integer;
			Boolean = value != 0;
			Integer = value;
			Number = value;
		}

		public ExpressionResult(float value)
		{
			Type = ExpressionResultType.Number;
			Boolean = value != 0.0f;
			Integer = (int)value;
			Number = value;
		}

		public ExpressionResultType Type;
		public bool Boolean;
		public int Integer;
		public float Number;
	}

	public class ExpressionEvaluationException : Exception
	{
		public ExpressionEvaluationException(Expression expression, string error) : base(string.Format("Error executing expression '{0}': {1}", expression, error))
		{
			Expression = expression;
			Error = error;
		}

		public Expression Expression;
		public string Error;
	}

	public abstract class Expression
	{
		public abstract ExpressionResult Evaluate(InstructionContext context);

		public override string ToString()
		{
			var builder = new StringBuilder();
			ToString(builder);
			return builder.ToString();
		}

		public abstract void ToString(StringBuilder builder);
	}
}
