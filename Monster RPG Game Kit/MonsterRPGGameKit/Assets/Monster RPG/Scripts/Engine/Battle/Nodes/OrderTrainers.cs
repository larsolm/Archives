using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class SortConditionList : SerializedList<VariableReference> { }

	[CreateInstructionGraphNodeMenu("Battle/Order Trainers", 30)]
	[HelpURL(MonsterRpg.DocumentationUrl + "order-trainers")]
	public class OrderTrainers : InstructionGraphNode
	{
		private const string _invalidVariablesWarning = "(BOAII) Unable to order actions for {0}: the given variables must be a IIndexedVariableStore";

		[Tooltip("The node to go to when this node is finished")]
		public InstructionGraphNode Next;

		[Tooltip("The variables that will determine the priority of each Trainer (higher values will be ordered first)")]
		[ListDisplay(EmptyText = "Add variabless to determine trainer priority", AllowCollapse = false)]
		public SortConditionList SortConditions = new SortConditionList();

		public override Color NodeColor => Colors.ExecutionDark;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			foreach (var condition in SortConditions)
			{
				if (InstructionStore.IsInput(condition))
					inputs.Add(VariableDefinition.Create(condition.RootName, VariableType.Empty));
			}
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is TrainerContextList trainers)
			{
				trainers.List.Sort((left, right) =>
				{
					foreach (var condition in SortConditions)
					{
						var leftOrder = condition.GetValue(left);
						var rightOrder = condition.GetValue(right);

						if (leftOrder != rightOrder)
							return rightOrder.CompareTo(leftOrder); // right.CompareTo(left) for sorting in descending order
					}

					return 0;
				});
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidVariablesWarning, Name);
			}

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
