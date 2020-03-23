using PiRhoSoft.CompositionEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Ecosystem/Give Creature", 10)]
	[HelpURL(MonsterRpg.DocumentationUrl + "give-creature")]
	public class GiveCreature : InstructionGraphNode
	{
		private const string _creatureNotFoundWarning = "(ECGCCNF) Unable to give creature for {0}: the creature could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The Creature to give")]
		public CreatureReference Creature = new CreatureReference();

		public override Color NodeColor => Colors.ExecutionDark;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			if (Creature.Generator != null)
			{
				foreach (var input in Creature.Generator.Inputs)
				{
					if (InstructionStore.IsInput(input))
						inputs.Add(input.Definition);
				}
			}
		}

		public override void GetOutputs(List<VariableDefinition> outputs)
		{
			if (Creature.Generator != null)
			{
				foreach (var output in Creature.Generator.Outputs)
				{
					if (InstructionStore.IsOutput(output))
						outputs.Add(output.Definition);
				}
			}
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Creature.Species != null || Creature.Creature != null)
				Give(Creature);
			else
				Debug.LogWarningFormat(this, _creatureNotFoundWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}

		private void Give(CreatureReference reference)
		{
			var creature = Creature.CreateCreature(Player.Instance.Trainer);
			Player.Instance.Trainer.Roster.AddCreature(creature);
		}
	}
}
