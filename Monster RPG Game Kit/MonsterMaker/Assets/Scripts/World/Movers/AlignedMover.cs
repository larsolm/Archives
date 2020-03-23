using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Movers/Aligned Mover")]
	public class AlignedMover : CollisionMover
	{
		public override Vector2 MoveDirection { get { return _moveDirection; } }

		private Vector2Int _moveDirection = Vector2Int.down;

		public override void UpdateMove(float horizontal, float vertical)
		{
			var direction = Vector2.zero;

			if (horizontal < 0) direction.x = -1;
			if (horizontal > 0) direction.x = 1;
			if (vertical > 0) direction.y = 1;
			if (vertical < 0) direction.y = -1;

			if (direction.x != 0 && direction.y != 0)
				direction.Normalize();

			if (direction.x != 0 || direction.y != 0)
				_moveDirection = Vector2Int.RoundToInt(direction);

			_velocity = direction * MoveSpeed;

			Move();
		}
	}
}
