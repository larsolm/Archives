using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Inventory/Purchase Item", 21)]
	[HelpURL(MonsterRpg.DocumentationUrl + "purchase-item")]
	public class PurchaseItem : InstructionGraphNode
	{
		private const string _itemNotFoundWarning = "(EIPIINF) Unable to purchase item for {0}: the given variables must be a ShopItem";

		[Tooltip("The node to follow once the item has been purchased")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.ExecutionDark;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is ShopItem item)
				item.Purchase(Player.Instance.Trainer.Inventory);
			else
				Debug.LogErrorFormat(this, _itemNotFoundWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
