using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Inventory/Can Toss Item", 30)]
	[HelpURL(MonsterRpg.DocumentationUrl + "can-toss-item")]
	public class CanTossItem : InstructionGraphNode
	{
		private const string _itemNotFoundWarning = "(EICTIINF) Unable to check tossability item for {0}: the given variables must be an InventoryItem";

		[Tooltip("The node to follow if the item can be tossed")]
		public InstructionGraphNode OnTrue = null;

		[Tooltip("The node to follow if the item can't be tossed")]
		public InstructionGraphNode OnFalse = null;

		public override Color NodeColor => Colors.Branch;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is InventoryItem item)
			{
				if (item.Item.Type != ItemType.Key)
					graph.GoTo(OnTrue, variables.This, nameof(OnTrue));
				else
					graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}
			else
			{
				Debug.LogWarningFormat(this, _itemNotFoundWarning, Name);
				graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}

			yield break;
		}
	}
}
