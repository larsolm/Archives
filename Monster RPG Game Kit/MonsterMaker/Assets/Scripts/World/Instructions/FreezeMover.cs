using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class FreezeMover : Instruction
	{
		[Tooltip("The target mover to freeze.")] public VariableReference Target;

		public override void Begin(InstructionContext context)
		{
			var target = context.GetObject<MoveController>(Target);

			if (target != null)
				target.Freeze();
			else
				Debug.LogFormat("unable to find the controller {0} to freeze", Target.Name);
		}
	}
}
