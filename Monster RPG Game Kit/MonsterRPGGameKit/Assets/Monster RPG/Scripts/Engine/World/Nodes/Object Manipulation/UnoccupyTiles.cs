using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Unoccupy Tiles", 81)]
	[HelpURL(MonsterRpg.DocumentationUrl + "unoccupy-tiles")]
	public class UnoccupyTiles : InstructionGraphNode
	{
		private const string _missingColliderWarning = "(WUTMC) Unable to unoccupy tiles for {0}: the given variables must be StaticCollider";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingDark;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is StaticCollider target)
				target.UnoccupyCurrentTiles();
			else
				Debug.LogWarningFormat(this, _missingColliderWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
