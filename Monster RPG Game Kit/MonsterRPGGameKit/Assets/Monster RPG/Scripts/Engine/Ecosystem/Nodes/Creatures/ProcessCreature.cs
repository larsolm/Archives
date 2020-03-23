using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Ecosystem/Process Creature", 1)]
	[HelpURL(MonsterRpg.DocumentationUrl + "process-creature")]
	public class ProcessCreature : InstructionGraphNode, ILoopNode
	{
		private const string _invalidCreatureWarning = "(ECPCIC) Unable to process creature for {0}: the given variables must be a Creature";

		[Tooltip("The node to move to for processing each skill")]
		public InstructionGraphNode ProcessSkill = null;

		public override Color NodeColor => Colors.ExecutionDark;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Creature creature)
			{
				creature.UpdatePendingTraits();

				if (creature.HasPendingSkill())
					graph.GoTo(ProcessSkill, variables.This, nameof(ProcessSkill));
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidCreatureWarning, name);
			}

			yield break;
		}
	}
}
