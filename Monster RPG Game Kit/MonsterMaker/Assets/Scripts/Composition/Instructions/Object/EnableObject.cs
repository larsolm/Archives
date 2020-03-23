using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class EnableObject : Instruction
	{
		[Tooltip("The target object to enable")] public VariableReference Target;

		public override void Begin(InstructionContext context)
		{
			var target = context.GetObject<GameObject>(Target);

			if (target != null)
				target.SetActive(true);
			else
				Debug.LogFormat("unable to find the object {0} to enable", Target.Name);
		}
	}
}
