using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Ecosystem/Release Creature", 11)]
	[HelpURL(MonsterRpg.DocumentationUrl + "release-creature")]
	public class ReleaseCreature : InstructionGraphNode
	{
		private const string _missingCreatureWarning = "(ERLCMC) Unable to release creature for {0}: the given variables must be a Creature";

		[Tooltip("The node to go to when the Creature is successfully released")]
		public InstructionGraphNode ReleasedNode;

		[Tooltip("The message to display when the Creature cannot be released")]
		public InstructionGraphNode CantReleaseNode;

		public override Color NodeColor => Colors.ExecutionDark;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Creature creature)
			{
				if (creature.Trainer != null && creature.Trainer.Roster.Count > 1)
				{
					creature.Trainer.Roster.RemoveCreature(creature);

					graph.GoTo(ReleasedNode, variables.This, nameof(ReleasedNode));
				}
				else
				{
					graph.GoTo(CantReleaseNode, variables.This, nameof(ReleasedNode));
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _missingCreatureWarning, Name);
				graph.GoTo(CantReleaseNode, variables.This, nameof(CantReleaseNode));
			}

			yield break;
		}
	}
}
