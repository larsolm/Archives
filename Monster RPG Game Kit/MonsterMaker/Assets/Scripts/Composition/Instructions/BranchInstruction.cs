using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class BranchInstruction<T> : Instruction
	{
		[Tooltip("The variable holding the value to branch on")] public VariableReference Variable;
		[Tooltip("The instruction to run if no instruction is specified for the value")] public Instruction Default;
		[Tooltip("The instruction to run if the variable is unset")] public Instruction Unset;

		public override void Begin(InstructionContext context)
		{
			T value;
			Instruction instruction;
			var store = context.GetStore(Variable);

			if (store != null && store.TryGet(Variable.Name, out value))
				instruction = GetInstruction(value) ?? Default;
			else
				instruction = Unset;

			if (instruction != null)
				context.Run(instruction, context.Parent);
		}

		protected abstract Instruction GetInstruction(T value);
	}
}
