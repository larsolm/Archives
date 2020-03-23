using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Thaw Controller", 61)]
	[HelpURL(MonsterRpg.DocumentationUrl + "thaw-controller")]
	public class ThawController : InstructionGraphNode
	{
		private const string _controllerNotFoundWarning = "(WOMTCCNF) Unable to thaw controller for {0}: the given variables must be a Controller";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingDark;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Controller target)
				target.Thaw();
			else
				Debug.LogWarningFormat(this, _controllerNotFoundWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
