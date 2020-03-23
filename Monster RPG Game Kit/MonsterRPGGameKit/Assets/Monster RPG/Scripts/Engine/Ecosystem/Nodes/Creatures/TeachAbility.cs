using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Ecosystem/Teach Ability", 21)]
	[HelpURL(MonsterRpg.DocumentationUrl + "teach-ability")]
	public class TeachAbility : InstructionGraphNode
	{
		private const string _invalidVariablesWarning = "(ETAIV) Unable to teach ability for {0}: the given variables must be a Creature";
		private const string _noAbilityWarning = "(ETANA) Unable to teach ability for {0}: the ability could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The Ability to Teach")]
		[InlineDisplay(PropagateLabel = true)]
		public AbilityVariableSource Ability = new AbilityVariableSource();

		public override Color NodeColor => Colors.ExecutionDark;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Ability.GetInputs(inputs);
		}

		protected override sealed IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Creature creature)
			{
				if (Ability.TryGetValue(variables, this, out var ability))
					creature.Moves.Add(ability.CreateMove(creature));
				else
					Debug.LogWarningFormat(this, _noAbilityWarning, Name);
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidVariablesWarning, Name);
			}

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
