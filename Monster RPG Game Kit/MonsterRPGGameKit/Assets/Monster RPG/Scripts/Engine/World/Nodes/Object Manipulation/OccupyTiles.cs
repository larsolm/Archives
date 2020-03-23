using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Occupy Tiles", 80)]
	[HelpURL(MonsterRpg.DocumentationUrl + "occupy-tiles")]
	public class OccupyTiles : InstructionGraphNode
	{
		private const string _missingColliderWarning = "(WOTMC) Unable to occupy tiles for {0}: the given variables must be a StaticCollider";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingLight;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is StaticCollider target)
				target.OccupyCurrentTiles();
			else
				Debug.LogWarningFormat(this, _missingColliderWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
