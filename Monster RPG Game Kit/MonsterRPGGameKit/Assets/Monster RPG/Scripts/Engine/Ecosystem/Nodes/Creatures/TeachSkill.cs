using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Ecosystem/Teach Skill", 20)]
	[HelpURL(MonsterRpg.DocumentationUrl + "teach-skill")]
	public class TeachSkill : InstructionGraphNode
	{
		private const string _invalidVariablesWarning = "(ETSIV) Unable to teach skill for {0}: the given variables must be a Creature";
		private const string _noSkillWarning = "(ETSNS) Unable to teach skill for {0}: The Creature '{1}' had no skills to learn";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.ExecutionDark;

		protected override sealed IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Creature creature)
			{
				var skill = creature.TakePendingSkill();
				if (skill != null)
				{
					if (creature.CanLearnSkill(skill))
						yield return creature.TeachSkill(skill, variables.Context);
				}
				else
				{
					Debug.LogWarningFormat(this, _noSkillWarning, Name, creature.name);
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidVariablesWarning, Name);
			}

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
