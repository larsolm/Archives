using System.Collections.Generic;
using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public abstract class CreatureDisplayNode : InstructionGraphNode
	{
		private const string _noInterfaceWarning = "(BCDNNIF) Unable to get creature display for {0}: the interface '{1}' could not be found";
		private const string _noIndexWarning = "(BCDNNI) Unable to get creature display for {0}: the index could not be found";

		[Tooltip("The node to move to when this node is finished")]
		[HideInInspector]
		public InstructionGraphNode Next = null;

		[Tooltip("The name of the interface which has the creature display")]
		public string InterfaceName = "Battle";

		[Tooltip("The reference to the index of the display to use with the instruction")]
		public VariableReference IndexVariable = new VariableReference();

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			if (InstructionStore.IsInput(IndexVariable))
				inputs.Add(VariableDefinition.Create(IndexVariable.RootName, VariableType.Integer));
		}

		protected CreatureDisplay GetDisplay(IVariableStore variables)
		{
			var battle = InterfaceManager.Instance.GetInterface<BattleInterface>(InterfaceName);
			if (battle != null)
			{
				if (IndexVariable.GetValue(variables).TryGetInteger(out var index))
					return battle.GetCreatureDisplay(index);
				else
					Debug.LogWarningFormat(this, _noIndexWarning, Name);
			}
			else
			{
				Debug.LogWarningFormat(this, _noInterfaceWarning, Name, InterfaceName);
			}

			return null;
		}
	}
}
