using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Freeze Controller", 60)]
	[HelpURL(MonsterRpg.DocumentationUrl + "freeze-controller")]
	public class FreezeController : InstructionGraphNode
	{
		private const string _controllerNotFoundWarning = "(WOMFCCNF) Unable to freeze controller for {0}: the given variables must be a Controller";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingLight;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Controller target)
				target.Freeze();
			else
				Debug.LogWarningFormat(this, _controllerNotFoundWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
