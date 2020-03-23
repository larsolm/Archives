using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Movers/Grid Mover")]
	[RequireComponent(typeof(Rigidbody2D))]
	public class GridMover : Mover
	{
		[Range(0, 30)] [Tooltip("The amount of frames to wait after changing direction before you start walking")] public int DirectionDelayFrames = 3;

		public bool Moving { get; private set; }
		public Vector2Int TargetGridPosition { get; private set; }

		public override bool CanInteract { get { return !Moving; } }
		public override float Speed { get { return Moving ? MoveSpeed : 0.0f; } }
		public override Vector2 MoveDirection { get { return _moveDirection; } }

		protected override Vector2Int GridPosition
		{
			get { return Moving ? CurrentGridPosition : TargetGridPosition; }
		}

		private Vector2Int _moveDirection;
		private Rigidbody2D _body;

		private int _frameDelay = 0;
		private bool _changingDirection = false;

		public override void UpdateMove(float horizontal, float vertical)
		{
			var wasMoving = Moving;

			if (Moving)
			{
				var speed = Time.fixedDeltaTime * MoveSpeed;
				var targetPosition = TargetGridPosition + PositionOffset;
				var position = Vector2.MoveTowards(_body.position, targetPosition, speed);

				_body.MovePosition(position);

				if (position == targetPosition)
					Moving = false;
			}

			UpdateGridPositions();

			if (!Moving && !DelayForFrame)
			{
				if (_changingDirection)
					_frameDelay++;
				else
					_frameDelay = DirectionDelayFrames;

				if (!_changingDirection && !wasMoving)
				{
					var direction = new Vector2Int(horizontal == 0 ? 0 : Math.Sign(horizontal), vertical == 0 ? 0 : Math.Sign(vertical));
					if (direction != _moveDirection)
					{
						if (horizontal < 0) ChangeDirection(Vector2Int.left);
						else if (horizontal > 0) ChangeDirection(Vector2Int.right);
						else if (vertical > 0) ChangeDirection(Vector2Int.up);
						else if (vertical < 0) ChangeDirection(Vector2Int.down);
					}
				}

				if (_frameDelay >= DirectionDelayFrames)
				{
					_changingDirection = false;

					if (horizontal < 0) Move(Vector2Int.left);
					else if (horizontal > 0) Move(Vector2Int.right);
					else if (vertical > 0) Move(Vector2Int.up);
					else if (vertical < 0) Move(Vector2Int.down);
				}
			}

			DelayForFrame = false;
		}

		public override void WarpToPosition(Vector2Int position, Vector2 direction, CollisionLayer layer)
		{
			base.WarpToPosition(position, direction, layer);

			Moving = false;
			TargetGridPosition = position;

			_moveDirection = Vector2Int.RoundToInt(direction.normalized);
		}

		public override void UnoccupyCurrentTiles()
		{
			if (CurrentZone != null)
				CurrentZone.Tilemap.SetUnoccupied(CurrentGridPosition, CollisionLayer);
		}

		public override void OccupyCurrentTiles()
		{
			if (CurrentZone != null)
				CurrentZone.Tilemap.SetOccupied(CurrentGridPosition, CollisionLayer);
		}

		protected virtual void Awake()
		{
			var x = Mathf.RoundToInt(transform.position.x - PositionOffset.x);
			var y = Mathf.RoundToInt(transform.position.y - PositionOffset.y);

			Moving = false;
			TargetGridPosition = new Vector2Int(x, y);

			ChangeDirection(Vector2Int.down);

			_body = GetComponent<Rigidbody2D>();
			_body.bodyType = RigidbodyType2D.Kinematic;
			_body.interpolation = RigidbodyInterpolation2D.Interpolate;
		}

		protected override void TileLeft(Vector2Int position)
		{
			base.TileLeft(position);

			if (CurrentZone != null)
				CurrentZone.Tilemap.SetUnoccupied(position, CollisionLayer);
		}

		protected override void TileEntered(Vector2Int position)
		{
			base.TileEntered(position);

			if (CurrentZone != null)
				CurrentZone.Tilemap.SetOccupied(position, CollisionLayer);
		}

		private void ChangeDirection(Vector2Int direction)
		{
			_changingDirection = true;
			_frameDelay = 0;
			_moveDirection = direction;
		}

		private void Move(Vector2Int direction)
		{
			if (GetCanMove(direction))
			{
				Moving = true;
				TargetGridPosition += direction;

				if (CurrentZone != null)
					CurrentZone.Tilemap.SetOccupied(TargetGridPosition, CollisionLayer);
			}
			
			_moveDirection = direction;
		}

		private bool GetCanMove(Vector2Int direction)
		{
			if (CurrentZone != null)
			{
				var position = CurrentGridPosition + direction;
				var tile = CurrentZone.Tilemap.GetTile(position);
				return tile == null || !tile.IsOccupiedOrHasCollision(CollisionLayer);
			}

			return true;
		}
	}
}
