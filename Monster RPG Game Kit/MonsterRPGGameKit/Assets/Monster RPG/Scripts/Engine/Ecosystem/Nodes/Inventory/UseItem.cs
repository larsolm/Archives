using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Inventory/Use Item", 11)]
	[HelpURL(MonsterRpg.DocumentationUrl + "use-item")]
	public class UseItem : InstructionGraphNode
	{
		private const string _itemNotFoundWarning = "(EIUIINF) Unable to use item for {0}: the given variables must be an Item or an Inventory Item";

		[Tooltip("Whether this item will be used in the World or in Battle")]
		[EnumButtons]
		public ItemUseLocation Location;

		[Tooltip("The node to follow after the item is used")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.ExecutionLight;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Item item)
				yield return Use(variables, item);
			else if (variables.This is InventoryItem inventoryItem)
				yield return Use(variables, inventoryItem);
			else
				Debug.LogWarningFormat(this, _itemNotFoundWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));
		}

		private IEnumerator Use(InstructionStore variables, Item item)
		{
			if (Location == ItemUseLocation.World)
				yield return item.UseInWorld(variables);
			else if (Location == ItemUseLocation.Battle)
				yield return item.UseInBattle(variables);
		}

		private IEnumerator Use(InstructionStore variables, InventoryItem item)
		{
			if (Location == ItemUseLocation.World)
				yield return item.UseInWorld(variables);
			else if (Location == ItemUseLocation.Battle)
				yield return item.UseInBattle(variables);
		}
	}
}
