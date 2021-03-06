﻿using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Animation/Play Animation State")]
	[HelpURL(Composition.DocumentationUrl + "play-animation-state")]
	public class PlayAnimationState : InstructionGraphNode
	{
		private const string _stateNotFoundWarning = "(CAPASSNF) Unable to play animation state on {0}: the state could not be found";
		private const string _animatorNotFoundWarning = "(CAPASANF) Unable to play animation state on {0}: the given variables must be an Animator";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The animation state to play")]
		[InlineDisplay(PropagateLabel = true)]
		public StringVariableSource State = new StringVariableSource();

		public override Color NodeColor => Colors.Animation;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			State.GetInputs(inputs);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Animator animator)
			{
				if (State.TryGetValue(variables, this, out var state))
					animator.Play(state);
				else
					Debug.LogWarningFormat(this, _stateNotFoundWarning, Name);
			}
			else
			{
				Debug.LogWarningFormat(this, _animatorNotFoundWarning, Name);
			}

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
