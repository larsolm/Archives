using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class CommandExpression : Expression
	{
		public string Name;
		public List<Expression> Parameters;

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Name);
			builder.Append('(');

			for (var i = 0; i < Parameters.Count; i++)
			{
				if (i != 0)
					builder.Append(", ");

				Parameters[i].ToString(builder);
			}

			builder.Append(')');
		}
	}
}
