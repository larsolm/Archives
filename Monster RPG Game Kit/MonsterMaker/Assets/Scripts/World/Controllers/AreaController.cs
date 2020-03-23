using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Move Controllers/Area Move Controller")]
	[RequireComponent(typeof(GridMover))] // THIS COULD EVENTUALLY BE MODIFIED TO WORK ON OTHER MOVER TYPES AS WELL.
	public class AreaController : MoveController
	{
		private enum MoveDirection
		{
			Left,
			Right,
			Up,
			Down,
			None
		}

		[Tooltip("The left distance this character can move from its starting point. Must be greater than or equal to 0.")] public int LeftDistance = 0;
		[Tooltip("The right distance this character can move from its starting point. Must be greater than or equal to 0.")] public int RightDistance = 0;
		[Tooltip("The up distance this character can move from its starting point. Must be greater than or equal to 0.")] public int UpDistance = 0;
		[Tooltip("The down distance this character can move from its starting point. Must be greater than or equal to 0.")] public int DownDistance = 0;
		[Tooltip("The delay before this character moves to a new tile. Must be greater than or equal to 0.")] public float MovementDelay = 1.0f;

		private GridMover _gridMover;
		private RectInt _area;
		private float _movementDelay = 0.0f;

		void Start()
		{
			_gridMover = Mover as GridMover;
			_gridMover.OnTileChanged += OnTileChanged;
			_area = new RectInt(_gridMover.CurrentGridPosition.x - LeftDistance, _gridMover.CurrentGridPosition.y - DownDistance, 1 + RightDistance + LeftDistance, 1 + UpDistance + DownDistance);
		}

		void FixedUpdate()
		{
			var horizontal = 0.0f;
			var vertical = 0.0f;

			if (!_gridMover.Moving)
			{
				_movementDelay += Time.fixedDeltaTime;

				if (_movementDelay >= MovementDelay)
				{
					var direction = GetMoveDirection();
					if (direction == MoveDirection.Left) horizontal = -1.0f;
					if (direction == MoveDirection.Right) horizontal = 1.0f;
					if (direction == MoveDirection.Up) vertical = 1.0f;
					if (direction == MoveDirection.Down) vertical = -1.0f;
					if (direction == MoveDirection.None) { horizontal = 0.0f; vertical = 0.0f; }
				}
			}

			UpdateMover(horizontal, vertical);
		}

		private void OnTileChanged(Vector2Int previous, Vector2Int current)
		{
			_movementDelay = 0.0f;
		}

		private MoveDirection GetMoveDirection()
		{
			var direction = (MoveDirection)Random.Range(0, (int)MoveDirection.None);

			if (direction == MoveDirection.Left && _gridMover.CurrentGridPosition.x > _area.xMin) return direction;
			if (direction == MoveDirection.Right && _gridMover.CurrentGridPosition.x < _area.xMax - 1) return direction;
			if (direction == MoveDirection.Up && _gridMover.CurrentGridPosition.y < _area.yMax - 1) return direction;
			if (direction == MoveDirection.Down && _gridMover.CurrentGridPosition.y > _area.yMin) return direction;

			return MoveDirection.None;
		}
	}
}
