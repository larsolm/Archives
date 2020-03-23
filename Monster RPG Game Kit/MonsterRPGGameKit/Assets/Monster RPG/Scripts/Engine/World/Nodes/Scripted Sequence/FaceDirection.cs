using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Scripted Sequence/Face Direction", 10)]
	[HelpURL(MonsterRpg.DocumentationUrl + "face-direction")]
	public class FaceDirection : InstructionGraphNode
	{
		private const string _moverNotFoundWarning = "(WSSFDMNF) Unable to face direction for {0}: the given variables must be a Mover";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The direction for the mover to face")]
		[EnumButtons]
		public MovementDirection Direction;

		public override Color NodeColor => Colors.Sequencing;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Mover target)
				target.FaceDirection(Direction);
			else
				Debug.LogWarningFormat(this, _moverNotFoundWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
