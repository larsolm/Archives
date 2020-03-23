using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public enum MovementDirection
	{
		None,
		Left,
		Right,
		Down,
		Up
	}

	[Flags]
	public enum InteractionDirection
	{
		Any = 0,
		Left = 1 << 0,
		Right = 1 << 1,
		Down = 1 << 2,
		Up = 1 << 3,
		This = 1 << 4
	}

	public static class Direction
	{
		public static bool Contains(InteractionDirection interactionDirection, MovementDirection movementDirection)
		{
			// If the movement direction that is sent in is None it means that the interaction is on the current tile, not the one the player is facing
			var direction = ToInteractionDirection(movementDirection);

			return (interactionDirection == InteractionDirection.Any && direction != InteractionDirection.This)
				|| (interactionDirection == InteractionDirection.This && direction == InteractionDirection.This)
				|| (((int)direction & (int)interactionDirection) > 0);
		}

		public static MovementDirection Opposite(MovementDirection direction)
		{
			switch (direction)
			{
				case MovementDirection.Left: return MovementDirection.Right;
				case MovementDirection.Right: return MovementDirection.Left;
				case MovementDirection.Down: return MovementDirection.Up;
				case MovementDirection.Up: return MovementDirection.Down;
				default: return MovementDirection.None;
			}
		}

		private static InteractionDirection ToInteractionDirection(MovementDirection direction)
		{
			switch (direction)
			{
				case MovementDirection.Left: return InteractionDirection.Left;
				case MovementDirection.Right: return InteractionDirection.Right;
				case MovementDirection.Down: return InteractionDirection.Down;
				case MovementDirection.Up: return InteractionDirection.Up;
				default: return InteractionDirection.This;
			}
		}

		public static Vector2Int GetVector(MovementDirection direction)
		{
			switch (direction)
			{
				case MovementDirection.Left: return Vector2Int.left;
				case MovementDirection.Right: return Vector2Int.right;
				case MovementDirection.Down: return Vector2Int.down;
				case MovementDirection.Up: return Vector2Int.up;
				default: return Vector2Int.zero;
			}
		}

		public static MovementDirection GetDirection(float horizontal, float vertical)
		{
			var direction = new Vector2Int(horizontal == 0 ? 0 : Math.Sign(horizontal), vertical == 0 ? 0 : Math.Sign(vertical));
			return GetDirection(direction);
		}

		public static MovementDirection GetDirection(Vector2Int direction)
		{
			if (direction == Vector2Int.left) return MovementDirection.Left;
			else if (direction == Vector2Int.right) return MovementDirection.Right;
			else if (direction == Vector2Int.down) return MovementDirection.Down;
			else if (direction == Vector2Int.up) return MovementDirection.Up;
			else return MovementDirection.None;
		}

		public static void GetMovement(MovementDirection direction, out float horizontal, out float vertical)
		{
			horizontal = 0.0f;
			vertical = 0.0f;

			if (direction == MovementDirection.Left) horizontal = -1.0f;
			if (direction == MovementDirection.Right) horizontal = 1.0f;
			if (direction == MovementDirection.Up) vertical = 1.0f;
			if (direction == MovementDirection.Down) vertical = -1.0f;
		}
	}
}
