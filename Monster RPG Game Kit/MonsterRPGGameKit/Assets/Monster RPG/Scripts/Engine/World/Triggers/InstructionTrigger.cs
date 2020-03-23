using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class InstructionTrigger : IInteractable
	{
		[Tooltip("The directions from which the interaction instructions can be triggered")][EnumButtons] public InteractionDirection InteractionDirections;
		[Tooltip("The directions from which the entering instructions can be triggered")] [EnumButtons] public InteractionDirection EnteringDirections;
		[Tooltip("The directions from which the enter instructions can be triggered")] [EnumButtons] public InteractionDirection EnterDirections;
		[Tooltip("The directions in which the exiting instructions can be triggered")] [EnumButtons] public InteractionDirection ExitingDirections;
		[Tooltip("The directions in which the exit instructions can be triggered")] [EnumButtons] public InteractionDirection ExitDirections;

		[Tooltip("The instructions to run when this tile is interacted with")] public InstructionCaller InteractionInstructions = new InstructionCaller();
		[Tooltip("The instructions to run when this tile is going to be entered")] public InstructionCaller EnteringInstructions = new InstructionCaller();
		[Tooltip("The instructions to run when this tile is fully entered")] public InstructionCaller EnterInstructions = new InstructionCaller();
		[Tooltip("The instructions to run when this tile is going to be exited")] public InstructionCaller ExitingInstructions = new InstructionCaller();
		[Tooltip("The instructions to run when this tile is fully exited")] public InstructionCaller ExitInstructions = new InstructionCaller();

		public static bool operator == (InstructionTrigger first, InstructionTrigger second)
		{
			return first.EnteringInstructions == second.EnteringInstructions
				&& first.EnterInstructions == second.EnterInstructions
				&& first.ExitingInstructions == second.ExitingInstructions
				&& first.ExitInstructions == second.ExitInstructions
				&& first.InteractionInstructions == second.InteractionInstructions;
		}

		public static bool operator != (InstructionTrigger first, InstructionTrigger second)
		{
			return !(first == second);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool IsInteracting()
		{
			return InteractionInstructions != null && InteractionInstructions.IsRunning;
		}

		public bool CanInteract(MovementDirection direction)
		{
			return Direction.Contains(InteractionDirections, Direction.Opposite(direction)) && !IsInteracting();
		}

		public void Interact()
		{
			Trigger(InteractionInstructions);
		}

		public void Entering(MovementDirection direction)
		{
			Trigger(EnteringDirections, EnteringInstructions, Direction.Opposite(direction));
		}

		public void Enter(MovementDirection direction)
		{
			Trigger(EnterDirections, EnterInstructions, Direction.Opposite(direction));
		}

		public void Exiting(MovementDirection direction)
		{
			Trigger(ExitingDirections, ExitingInstructions, direction); // Don't flip this direction because it is the same way the player is facing
		}

		public void Exit(MovementDirection direction)
		{
			Trigger(ExitDirections, ExitInstructions, direction); // Don't flip this direction because it is the same way the player is facing
		}

		private void Trigger(InteractionDirection interactionDirection, InstructionCaller instruction, MovementDirection direction)
		{
			if (Direction.Contains(interactionDirection, direction))
				Trigger(instruction);
		}

		private void Trigger(InstructionCaller instruction)
		{
			if (instruction != null && instruction.Instruction != null)
				InstructionManager.Instance.RunInstruction(instruction, WorldManager.Instance.Context, Player.Instance);
		}
	}
}
