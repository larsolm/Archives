using System;
using System.Collections;
using System.Collections.Generic;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class AiSelectionItem
	{
		[Tooltip("The node to go to when this item is selected")] public InstructionGraphNode OnSelected;
		[Tooltip("The label used to identify this item")] public string Label;
		[Tooltip("The variables to use for this item (these will be expanded if they are an IIndexedVariableStore)")] public VariableReference Variables = new VariableReference("this");
	}

	[Serializable]
	public class AiSelectionItemList : SerializedList<AiSelectionItem> { }

	[CreateInstructionGraphNodeMenu("Battle/Ai Selection", 31)]
	[HelpURL(Composition.DocumentationUrl + "ai-selection")]
	public class AiSelection : InstructionGraphNode
	{
		private const string _invalidVariablesWarning = "(BASIV) Unable to make ai selection for {0}: the given variables must be a BattleAi";

		[Tooltip("A optional tag to send to the Battle AI so that it knows what it is selecting from")]
		public string Tag;

		[Tooltip("The items that the ai can select from")]
		[ListDisplay(ItemDisplay = ListItemDisplayType.Foldout, EmptyText = "Add items to create selection options")]
		public AiSelectionItemList Items = new AiSelectionItemList();

		private List<VariableValue> _values = new List<VariableValue>();
		private List<int> _indexMap = new List<int>();

		public override Color NodeColor => Colors.InterfaceTeal;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			foreach (var item in Items)
			{
				if (InstructionStore.IsInput(item.Variables))
					inputs.Add(VariableDefinition.Create(item.Variables.RootName, VariableType.Empty));
			}
		}

		public override void GetConnections(NodeData data)
		{
			foreach (var item in Items)
				data.AddConnection(nameof(Items), item.Label, item.OnSelected);

			base.GetConnections(data);
		}

		public override void SetConnection(ConnectionData connection, InstructionGraphNode target)
		{
			if (connection.Field == nameof(Items))
			{
				foreach (var item in Items)
				{
					if (item.Label == connection.FieldKey)
					{
						item.OnSelected = target;
						return;
					}
				}
			}
			else
			{
				base.SetConnection(connection, target);
			}
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is BattleAi ai)
			{
				var values = CreateValues(variables);
				var selection = ai.MakeSelection(variables, values, Tag);
				var index = _indexMap[selection];
				var selected = Items[index];
				var value = values[index];

				if (value.TryGetStore(out var store))
					graph.GoTo(selected.OnSelected, store, selected.Label);
				else
					graph.GoTo(selected.OnSelected, variables.This, selected.Label);
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidVariablesWarning, Name);
			}

			yield break;
		}

		private List<VariableValue> CreateValues(InstructionStore variables)
		{
			_values.Clear();
			_indexMap.Clear();

			for (var i = 0; i < Items.Count; i++)
			{
				var item = Items[i];

				if (item.Variables.IsAssigned)
				{
					var value = item.Variables.GetValue(variables);
					if (value.TryGetStore(out var store) && (store is IIndexedVariableStore indexed))
					{
						for (var index = 0; index < indexed.Count; index++)
						{
							_values.Add(VariableValue.Create(indexed.GetItem(index)));
							_indexMap.Add(i);
						}
					}
					else
					{
						_values.Add(value);
						_indexMap.Add(i);
					}
				}
				else
				{
					_values.Add(VariableValue.Create(item.Label));
					_indexMap.Add(i);
				}
			}

			return _values;
		}
	}
}
