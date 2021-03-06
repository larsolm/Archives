﻿using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class LookupOperation : Operation
	{
		public VariableReference Reference = new VariableReference();

		public LookupOperation(string variable)
		{
			Reference.Update(variable);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			return Reference.GetValue(variables);
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(Reference.ToString());
		}

		public override void GetInputs(List<VariableDefinition> inputs, string source)
		{
			GetVariables(inputs, source);
		}

		public override void GetOutputs(List<VariableDefinition> outputs, string source)
		{
			GetVariables(outputs, source);
		}

		private void GetVariables(List<VariableDefinition> inputs, string source)
		{
			if (source == null)
			{
				if (Reference.IsAssigned && string.IsNullOrEmpty(Reference.StoreName))
					inputs.Add(VariableDefinition.Create(Reference.RootName, VariableType.Empty));
			}
			else if (Reference.IsAssigned && Reference.StoreName == source)
			{
				inputs.Add(VariableDefinition.Create(Reference.RootName, VariableType.Empty));
			}
		}
	}
}
