using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Battle/Play Battle Sound", 2)]
	[HelpURL(MonsterRpg.DocumentationUrl + "play-battle-sound")]
	public class PlayBattleSound : CreatureDisplayNode
	{
		[InlineDisplay]
		public BattleAnimationSound Sound = new BattleAnimationSound();

		public override Color NodeColor => Colors.Animation;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var display = GetDisplay(variables);

			if (display != null)
			{
				if (Sound.WaitForCompletion)
					yield return display.PlayAndWait(Sound, variables);
				else
					display.Play(Sound, variables);
			}

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
