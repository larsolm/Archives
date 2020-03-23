using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Warp Mover", 100)]
	[HelpURL(MonsterRpg.DocumentationUrl + "warp-mover")]
	public class WarpMover : InstructionGraphNode
	{
		private const string _moverNotFoundWarning = "(WOMWMMNF) Unable to warp mover for {0}: the given variables must be a Mover";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The position to warp the mover to")]
		public Vector2Int Position;

		[Tooltip("The direction for the mover to face")] [EnumButtons]
		public MovementDirection Direction;

		[Tooltip("The movement layer to put the mover on (None or All mean no change)")]
		public CollisionLayer Layer = CollisionLayer.One;

		public override Color NodeColor => Colors.Sequencing;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Mover target)
				target.WarpToPosition(Position, Direction, Layer);
			else
				Debug.LogWarningFormat(this, _moverNotFoundWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
