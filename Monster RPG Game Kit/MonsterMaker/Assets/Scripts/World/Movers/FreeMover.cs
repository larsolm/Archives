using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Movers/Free Mover")]
	public class FreeMover : CollisionMover
	{
		public override Vector2 MoveDirection { get { return _moveDirection; } }

		private Vector2 _moveDirection = Vector2.down;

		public override void UpdateMove(float horizontal, float vertical)
		{
			if (horizontal != 0 || vertical != 0)
				_moveDirection.Set(horizontal, vertical);

			_velocity.Set(horizontal * MoveSpeed, vertical * MoveSpeed);

			Move();
		}
	}
}
