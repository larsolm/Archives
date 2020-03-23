using System.Text;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class IdentifierExpression : Expression
	{
		public IdentifierExpression(string variable)
		{
			Variable = new VariableReference();

			var dot = variable.LastIndexOf('.');

			if (dot < 0)
			{
				Variable.Location = VariableLocation.Context;
				Variable.Name = variable;
			}
			else
			{
				Variable.Location = VariableLocation.Custom;
				Variable.CustomSource = variable.Substring(0, dot);
				Variable.Name = variable.Substring(dot + 1);
			}
		}

		public VariableReference Variable { get; private set; }

		public override ExpressionResult Evaluate(InstructionContext context)
		{
			var store = context.GetStore(Variable);

			if (store != null)
			{
				bool boolean;
				int integer;
				float number;

				if (store.TryGet(Variable.Name, out boolean))
					return new ExpressionResult(boolean);
				else if (store.TryGet(Variable.Name, out integer))
					return new ExpressionResult(integer);
				else if (store.TryGet(Variable.Name, out number))
					return new ExpressionResult(number);
			}

			Debug.LogFormat("Unable to find variable {0} in expression", Variable.Name);

			return new ExpressionResult(false);
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Variable);
		}
	}
}
