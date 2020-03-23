using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Inventory/Give Item", 0)]
	[HelpURL(MonsterRpg.DocumentationUrl + "give-item")]
	public class GiveItem : InstructionGraphNode
	{
		private const string _itemMissingWarning = "(EGIIM) Unable to give item for {0}: the item could not be found";
		private const string _amountInvalidWarning = "(EGIAI) Attempted to give a negative amount of the item for {0}: defaulting to 1";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The item to give")]
		[InlineDisplay(PropagateLabel = true)]
		public ItemVariableSource Item = new ItemVariableSource();

		[Tooltip("The amount of the item to give")]
		[InlineDisplay(PropagateLabel = true)]
		public IntegerVariableSource Amount = new IntegerVariableSource(1);

		public override Color NodeColor => Colors.ExecutionDark;

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
				Player.Instance.Trainer.Inventory.Add(item, amount);
			else
				Debug.LogWarningFormat(this, _itemMissingWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
