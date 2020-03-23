using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Inventory/Can Purchase Item", 20)]
	[HelpURL(MonsterRpg.DocumentationUrl + "can-purchase-item")]
	public class CanPurchaseItem : InstructionGraphNode
	{
		private const string _itemNotFoundWarning = "(EIPISINF) Unable to check item purchasability for {0}: the given variables must be a ShopItem";

		[Tooltip("The node to follow if the item can be purchased")]
		public InstructionGraphNode OnTrue = null;

		[Tooltip("The node to follow if the item can't be purchased")]
		public InstructionGraphNode OnFalse = null;

		public override Color NodeColor => Colors.Branch;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is ShopItem item)
			{
				if (item.CanPurchase(Player.Instance.Trainer.Inventory))
					graph.GoTo(OnTrue, variables.This, nameof(OnTrue));
				else
					graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}
			else
			{
				Debug.LogErrorFormat(this, _itemNotFoundWarning, Name);
				graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}

			yield break;
		}
	}
}
