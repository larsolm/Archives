using PiRhoSoft.CompositionEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Ecosystem/Create Creature", 0)]
	[HelpURL(MonsterRpg.DocumentationUrl + "create-creature")]
	public class CreateCreature : InstructionGraphNode
	{
		private const string _invalidVariablesWarning = "(ECCIV) Unable to initialize creature for {0}: the given varibales must be a Creature";

		[Tooltip("The node to move to for proccessing the created creature")]
		public InstructionGraphNode ProcessCreature = null;

		[Tooltip("The Expression to run to initialize the Creature's traits")]
		public Expression Initializer = new Expression();

		public override Color NodeColor => Colors.ExecutionDark;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Initializer.GetInputs(inputs, InstructionStore.InputStoreName);
		}

		public override void GetOutputs(List<VariableDefinition> outputs)
		{
			Initializer.GetOutputs(outputs, InstructionStore.OutputStoreName);
		}

		protected override sealed IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Creature creature)
			{
				Initializer.Execute(this, creature);
				graph.GoTo(ProcessCreature, variables.This, nameof(ProcessCreature));
			}
			else
			{
				Debug.LogWarningFormat(this, _invalidVariablesWarning, Name);
				yield break;
			}
		}
	}
}
