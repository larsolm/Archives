using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Battle/Use Move", 21)]
	[HelpURL(MonsterRpg.DocumentationUrl + "use-move")]
	public class UseMove : InstructionGraphNode
	{
		private const string _invalidMoveWarning = "(BUMIM) Unable to use move for {0}: the given variables must be a MoveContext";

		[Tooltip("Whether this move will be used in the World or in Battle")]
		[EnumButtons]
		public AbilityUseLocation Location;

		[Tooltip("The node to follow after the move is used")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.ExecutionLight;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Move move)
				yield return Use(variables, move);
			else if (variables.This is MoveContext context)
				yield return Use(variables, context.Move);
			else
				Debug.LogWarningFormat(this, _invalidMoveWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));
		}

		private IEnumerator Use(InstructionStore variables, Move move)
		{
			if (Location == AbilityUseLocation.Battle)
				yield return move.Ability.UseInBattle(variables);
			else if (Location == AbilityUseLocation.World)
				yield return move.Ability.UseInWorld(variables);
		}
	}
}
