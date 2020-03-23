using PiRhoSoft.CompositionEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Scripted Sequence/Face Mover", 11)]
	[HelpURL(MonsterRpg.DocumentationUrl + "face-mover")]
	public class FaceMover : InstructionGraphNode
	{
		private const string _moverNotFoundWarning = "(WSSFMMNF) Unable to face mover for {0}: the given variables must be a Mover";
		private const string _moverTowardNotFoundWarning = "(WSSFMMTNF) Unable to face mover for {0}: the target '{1}' could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The mover to face")]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.Sequencing;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			if (InstructionStore.IsInput(Target))
				inputs.Add(VariableDefinition.Create<Mover>(Target.RootName));
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Mover target)
			{
				if (Target.GetValue(variables).TryGetObject(out Mover toward))
				{
					var dir = toward.CurrentGridPosition - target.CurrentGridPosition;
					var direction = Direction.GetDirection(dir.x, dir.y);

					target.FaceDirection(direction);
				}
				else
				{
					Debug.LogWarningFormat(this, _moverTowardNotFoundWarning, Name, Target);
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _moverNotFoundWarning, Name);
			}

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
