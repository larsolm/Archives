using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Battle/Play Battle Clip", 0)]
	[HelpURL(MonsterRpg.DocumentationUrl + "play-battle-clip")]
	public class PlayBattleClip : CreatureDisplayNode
	{
		[InlineDisplay]
		public BattleAnimationClip Clip = new BattleAnimationClip();

		public bool IsExecutionImmediate => !Clip.WaitForCompletion;

		public override Color NodeColor => Colors.Animation;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var display = GetDisplay(variables);

			if (display != null)
			{
				if (Clip.WaitForCompletion)
					yield return display.PlayAndWait(Clip, variables);
				else
					display.Play(Clip, variables);
			}

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
