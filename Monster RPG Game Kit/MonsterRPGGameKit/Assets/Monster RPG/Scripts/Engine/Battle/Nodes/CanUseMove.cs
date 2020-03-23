using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Battle/Can Use Move", 20)]
	[HelpURL(MonsterRpg.DocumentationUrl + "can-use-move")]
	public class CanUseMove : InstructionGraphNode
	{
		private const string _invalidMoveWarning = "(BCUMIM) Unable to check usability of move for {0}: the given variables must be a Move or a MoveContext";

		[Tooltip("The location to check for if this move can be used in")]
		[EnumButtons]
		public AbilityUseLocation Location;

		[Tooltip("The node to follew if the move can be used")]
		public InstructionGraphNode OnTrue = null;

		[Tooltip("The node to follow if the move cannot be use")]
		public InstructionGraphNode OnFalse = null;

		public override Color NodeColor => Colors.Branch;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Move move)
			{
				Use(graph, variables, move);
			}
			else if (variables.This is MoveContext context)
			{
				Use(graph, variables, context.Move);
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidMoveWarning, Name);
				graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}

			yield break;
		}

		private void Use(InstructionGraph graph, InstructionStore variables, Move move)
		{
			if (Location == AbilityUseLocation.Battle)
			{
				if (move.Ability.IsUsableInBattle(variables))
					graph.GoTo(OnTrue, variables.This, nameof(OnTrue));
				else
					graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}
			else if (Location == AbilityUseLocation.World)
			{
				if (move.Ability.IsUsableInWorld(variables))
					graph.GoTo(OnTrue, variables.This, nameof(OnTrue));
				else
					graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}
			else
			{
				graph.GoTo(OnFalse, variables.This, nameof(OnFalse));
			}
		}
	}
}
