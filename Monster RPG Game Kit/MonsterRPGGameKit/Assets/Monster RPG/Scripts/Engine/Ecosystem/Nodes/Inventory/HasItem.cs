using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Inventory/Has Item", 1)]
	[HelpURL(MonsterRpg.DocumentationUrl + "has-item")]
	public class HasItem : InstructionGraphNode
	{
		private const string _itemMissingWarning = "(EHIIM) Unable to check for item for {0}: the item could not be found";
		private const string _amountInvalidWarning = "(EHIAI) Attempted to check for a negative amount of item for {0}: defaulting to 1";

		[Tooltip("The node to follow if the player has the item")]
		public InstructionGraphNode OnTrue = null;

		[Tooltip("The node to follow if the player doesn't have the item")]
		public InstructionGraphNode OnFalse = null;

		[Tooltip("The item to check for")]
		[InlineDisplay(PropagateLabel = true)]
		public ItemVariableSource Item = new ItemVariableSource();

		[Tooltip("The amount of the item to check for")]
		[InlineDisplay(PropagateLabel = true)]
		public IntegerVariableSource Amount = new IntegerVariableSource(1);

		public override Color NodeColor => Colors.Branch;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Item.GetInputs(inputs);
			Amount.GetInputs(inputs);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			Item.TryGetValue(variables, this, out var item);
			Amount.TryGetValue(variables, this, out var amount);

			if (amount <= 0)
			{
				amount = 1;
				Debug.LogWarningFormat(this, _amountInvalidWarning, Name);
			}

			if (item)
			{
				if (Player.Instance.Trainer.Inventory.Contains(item, amount))
					graph.GoTo(OnTrue, variables.This, nameof(OnTrue));
				else
					graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}
			else
			{
				Debug.LogWarningFormat(this, _itemMissingWarning, Name);
				graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}

			yield break;
		}
	}
}
