using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Battle/Set Creature", 10)]
	[HelpURL(MonsterRpg.DocumentationUrl + "set-creature")]
	public class SetCreature : CreatureDisplayNode
	{
		private const string _invalidCreatureWarning = "(BUCIC) Unable to set creature display for {0}: this given variables must be a CreatureContext";

		public override Color NodeColor => Colors.InterfaceLight;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is CreatureContext creature)
			{
				var display = GetDisplay(variables);
				display?.SetCreature(creature);
				graph.GoTo(Next, variables.This, nameof(Next));
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidCreatureWarning, Name);
			}

			yield break;
		}
	}
}
