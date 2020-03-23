using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class SetState<T> : Instruction
	{
		[Tooltip("The variable to set")] public VariableReference Variable;
		[Tooltip("The value to set the variable to.")] public T Value;

		public override void Begin(InstructionContext context)
		{
			var store = context.GetStore(Variable);
			
			if (store != null)
				store.ChangeOrAdd(Variable.Name, Value);
			else
				Debug.LogFormat(this, "Variable store not found while attempting to set {0}", Variable.Name);
		}
	}
}
