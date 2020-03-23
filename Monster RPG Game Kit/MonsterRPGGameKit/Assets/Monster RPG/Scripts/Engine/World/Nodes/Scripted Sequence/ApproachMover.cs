using PiRhoSoft.CompositionEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Scripted Sequence/Approach Mover", 0)]
	[HelpURL(MonsterRpg.DocumentationUrl + "approach-mover")]
	public class ApproachMover : ApproachNode
	{
		private const string _moverNotFoundWarning = "(WSSAMMNF) Unable to approach mover for {0}: the target mover '{1}' could not be found";

		[Tooltip("The mover to move toward")]
		public VariableReference Target = new VariableReference();

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			if (InstructionStore.IsInput(Target))
				inputs.Add(VariableDefinition.Create<Mover>(Target.RootName));
		}

		protected override Vector2Int GetTargetPosition(InstructionStore variables)
		{
			if (Target.GetValue(variables).TryGetObject(out Mover toward))
				return toward.CurrentGridPosition;
			else
				Debug.LogWarningFormat(this, _moverNotFoundWarning, Name, Target);

			return Vector2Int.zero;
		}
	}
}
