﻿namespace PiRhoSoft.CompositionEngine
{
	public class LessOrEqualOperator : InfixOperation
	{
		public override VariableValue Evaluate(IVariableStore variables)
		{
			var left = Left.Evaluate(variables);
			var right = Right.Evaluate(variables);

			return VariableValue.Create(left <= right);
		}
	}
}
