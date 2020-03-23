using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Battle/Play Battle Effect", 1)]
	[HelpURL(MonsterRpg.DocumentationUrl + "play-battle-effect")]
	public class PlayBattleEffect : CreatureDisplayNode
	{
		[InlineDisplay]
		public BattleAnimationEffect Effect = new BattleAnimationEffect();

		public override Color NodeColor => Colors.Animation;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var display = GetDisplay(variables);

			if (display != null)
			{
				if (Effect.WaitForCompletion)
					yield return display.PlayAndWait(Effect, variables);
				else
					display.Play(Effect, variables);
			}

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
