using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class DestroyObject : Instruction
	{
		[Tooltip("The target object to destroy")] public VariableReference Target;

		public override void Begin(InstructionContext context)
		{
			var target = context.GetObject<GameObject>(Target);

			if (target != null)
				Destroy(target);
			else
				Debug.LogFormat("unable to find the object {0} to destroy", Target.Name);
		}
	}
}
