using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class ExpressionStatement : ISerializationCallbackReceiver
	{
		[SerializeField] private string _statement;
		private Expression _expression;

		public string Statement
		{
			get { return _statement; }
			set { _statement = value; Compile(); }
		}

		public void Compile()
		{
			var tokens = ExpressionLexer.Tokenize(_statement);
			_expression = ExpressionParser.Parse(tokens);
		}

		public ExpressionResult Execute(InstructionContext context)
		{
			if (_expression == null)
				throw new ExpressionEvaluationException(_expression, "expression is null");

			return _expression.Evaluate(context);
		}

		public void FindInputs(InstructionInputList inputs, VariableLocation location)
		{
			FindInputs(_expression, inputs, location);
		}

		private static void FindInputs(Expression expression, InstructionInputList inputs, VariableLocation location)
		{
			if (expression != null)
			{
				var identifier = expression as IdentifierExpression;
				if (identifier != null)
				{
					if (identifier.Variable != null && identifier.Variable.Location == location)
						inputs.AddPrimitive(identifier.Variable);

					return;
				}

				var command = expression as CommandExpression;
				if (command != null)
				{
					foreach (var parameter in command.Parameters)
						FindInputs(parameter, inputs, location);

					return;
				}

				var assign = expression as AssignOperator;
				if (assign != null)
				{
					FindInputs(assign.Right, inputs, location);
					return;
				}

				var infix = expression as InfixOperatorExpression;
				if (infix != null)
				{
					FindInputs(infix.Left, inputs, location);
					FindInputs(infix.Right, inputs, location);
					return;
				}

				var prefix = expression as PrefixOperatorExpression;
				if (prefix != null)
				{
					FindInputs(prefix.Right, inputs, location);
					return;
				}
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!string.IsNullOrEmpty(_statement))
				Compile();
		}
	}
}
