using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Battle/Update Creature", 11)]
	[HelpURL(MonsterRpg.DocumentationUrl + "update-creature")]
	public class UpdateCreature : CreatureDisplayNode
	{
		[Tooltip("The binding group to update")]
		public string Group;

		[Tooltip("Whether to wait for any binding animations to complete or not")]
		public bool WaitForCompletion = false;

		public override Color NodeColor => Colors.Interface;

		private BindingAnimationStatus _status = new BindingAnimationStatus();

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			_status.Reset();

			var display = GetDisplay(variables);
			display?.UpdateCreature(Group, _status);

			if (WaitForCompletion)
			{
				while (!_status.IsFinished())
					yield return null;
			}

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
