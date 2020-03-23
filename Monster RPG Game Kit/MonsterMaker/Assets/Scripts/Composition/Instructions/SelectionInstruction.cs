using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class SelectionOption
	{
		public Instruction Instruction;
	}

	public abstract class SelectionInstruction : PromptInstruction
	{
		[Tooltip("The message to show before showing the options")] public PromptString Message;
		[Tooltip("The variable to store the selection in (optional)")] public VariableReference Variable;
		[Tooltip("The instruction to run if the selection is canceled.")] public Instruction CancelInstruction;
	}

	public abstract class SelectionInstruction<T> : SelectionInstruction where T : SelectionOption
	{
		public abstract T[] Options { get; }

		public override void Begin(InstructionContext context)
		{
			context.Prompt(this, Options);
		}

		public override void OnIgnored(InstructionContext context)
		{
			if (Options.Length > 0)
				OnSelected(context, Options[0]);
			else
				context.Finish(this);
		}

		public override void OnDismissed(InstructionContext context)
		{
			context.Finish(this);

			if (CancelInstruction != null)
				context.Run(CancelInstruction, context.Parent);
		}

		public override void OnSelected(InstructionContext context, object selection)
		{
			var option = selection as SelectionOption;

			context.Finish(this);

			if (!string.IsNullOrEmpty(Variable.Name))
			{
				var store = context.GetStore(Variable);
				if (store != null)
					store.SetOrAdd(Variable.Name, selection as T);
			}

			if (option.Instruction != null)
				context.Run(option.Instruction, context.Parent);
		}
	}
}
