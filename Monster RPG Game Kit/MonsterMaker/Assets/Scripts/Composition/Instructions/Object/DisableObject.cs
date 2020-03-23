using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class DisableObject : Instruction
	{
		[Tooltip("The target object to disable")] public VariableReference Target;

		public override void Begin(InstructionContext context)
		{
			var target = context.GetObject<GameObject>(Target);

			if (target != null)
				target.SetActive(false);
			else
				Debug.LogFormat("unable to find the object {0} to disable", Target.Name);
		}
	}
}
