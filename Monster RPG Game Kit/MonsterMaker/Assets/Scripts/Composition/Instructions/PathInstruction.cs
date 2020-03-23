using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class PathInstruciton : Instruction
	{
		[Tooltip("The object to move.")] public VariableReference Target;
		[Tooltip("The path the object should follow move the object through.")] public Path Path;

		private Coroutine _move;

		public override void Begin(InstructionContext context)
		{
			var target = context.GetObject<MoveController>(Target);

			if (target != null)
				_move = target.StartCoroutine(Move(target));
			else
				Debug.LogFormat("unable to find the object {0} to move", Target.Name);
		}

		public override bool Execute(InstructionContext context)
		{
			return _move != null;
		}

		public override void End(InstructionContext context)
		{
			//var target = ComponentFinder.GetAsComponent<MoveController>(context.GetObject(Target));
			//if (target != null)
			//	target.Thaw();
			//else
			//	Debug.LogFormat("unable to find the object {0} to move", Target.Name);
		}

		private IEnumerator Move(MoveController controller)
		{
			controller.Freeze();

			controller.Mover.UpdateMove(0.0f, 0.0f);

			yield return null;

			controller.Thaw();
		}
	}
}
