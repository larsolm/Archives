using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Inventory/Can Use Item", 10)]
	[HelpURL(MonsterRpg.DocumentationUrl + "can-use-item")]
	public class CanUseItem : InstructionGraphNode
	{
		private const string _itemMissingWarning = "(ECUIIM) Unable to check usability of item for {0}: the given variables must be an Item or an InventoryItem";

		[Tooltip("The location to check for if this item can be used in")]
		[EnumButtons]
		public ItemUseLocation Location;

		[Tooltip("The node to follow if the item can be used")]
		public InstructionGraphNode OnTrue = null;

		[Tooltip("The node to follow if the item cannot be used")]
		public InstructionGraphNode OnFalse = null;

		public override Color NodeColor => Colors.Branch;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Item item)
			{
				Check(graph, variables, item);
			}
			else if (variables.This is InventoryItem inventoryItem)
			{
				Check(graph, variables, inventoryItem.Item);
			}
			else
			{
				Debug.LogWarningFormat(this, _itemMissingWarning, Name);
				graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}

			yield break;
		}

		private void Check(InstructionGraph graph, InstructionStore variables, Item item)
		{
			if (Location == ItemUseLocation.World)
			{
				if (item.IsUsableInWorld(variables))
					graph.GoTo(OnTrue, variables.This, nameof(OnTrue));
				else
					graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}
			else if (Location == ItemUseLocation.Battle)
			{
				if (item.IsUsableInBattle(variables))
					graph.GoTo(OnTrue, variables.This, nameof(OnTrue));
				else
					graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}
			else
			{
				graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}
		}
	}
}
