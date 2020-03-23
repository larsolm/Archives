using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Inventory/Toss Item", 31)]
	[HelpURL(MonsterRpg.DocumentationUrl + "toss-item")]
	public class TossItem : InstructionGraphNode
	{
		private const string _itemNotFoundWarning = "(EITIINF) Unable to toss item for {0}: the given variables must be an InventoryItem";

		[Tooltip("The node to follow if after the item has been tossed")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.ExecutionDark;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is InventoryItem item)
				item.Toss(item.Count);
			else
				Debug.LogWarningFormat(this, _itemNotFoundWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
