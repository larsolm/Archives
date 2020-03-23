using System;
using System.Collections;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class InstructionVariable
	{
		public string Name;
		public bool IsReference = false;
		public VariableValue Value = new VariableValue();
		public VariableReference Reference = new VariableReference();
	}

	[Serializable]
	public class InstructionCaller
	{
		public Instruction Instruction;
		public List<InstructionVariable> Inputs = new List<InstructionVariable>();
		public List<InstructionVariable> Outputs = new List<InstructionVariable>();

		public IEnumerator Run(InstructionContext context, UnityObject parent)
		{
			if (context.IsRunning)
				yield break;

			var frame = context.OpenFrame();
			WriteOutputs(context);
			WriteInputs(context);
			context.Run(Instruction, parent);
			yield return null;

			while (context.FrameCount > frame)
			{
				context.Update();
				yield return null;
			}

			ReadOutputs(context);
			context.CloseFrame();

			while (context.IsRunning)
			{
				context.Update();
				yield return null;
			}
		}

		private void WriteOutputs(InstructionContext context)
		{
			foreach (var input in Inputs)
				context.Variables.TryAddEmpty(input.Name);
		}

		private void WriteInputs(InstructionContext context)
		{
			foreach (var input in Inputs)
			{
				var variable = input.Value;

				if (input.IsReference)
				{
					var store = context.GetStore(input.Reference);
					variable = store != null ? store.GetVariable(input.Reference.Name) : null;
				}

				if (variable != null)
					context.Variables.Copy(input.Name, variable);
			}
		}

		private void ReadOutputs(InstructionContext context)
		{
			foreach (var output in Outputs)
			{
				var variable = context.Variables.GetVariable(output.Name);

				if (variable != null)
				{
					if (output.IsReference)
					{
						var store = context.GetStore(output.Reference);
						if (store != null)
							store.Copy(output.Reference.Name, variable);
					}
					else
					{
						output.Value = variable.Clone();
					}
				}
			}
		}
	}
}
