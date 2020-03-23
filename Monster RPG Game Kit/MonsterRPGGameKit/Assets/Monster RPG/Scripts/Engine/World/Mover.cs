using PiRhoSoft.UtilityEngine;
using UnityEngine;
using UnityEngine.Events;

namespace PiRhoSoft.MonsterRpgEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Renderer))]
	[HelpURL(MonsterRpg.DocumentationUrl + "mover")]
	[AddComponentMenu("PiRho Soft/World/Mover")]
	public class Mover : MonoBehaviour
	{
		public static readonly Vector2 PositionOffset = new Vector2(0.5f, 0.5f);
		private const int _shadowSortOffset = -10;

		[Tooltip("The movement layer this mover will start on and collide with")]
		public CollisionLayer MovementLayer = CollisionLayer.One;

		[Tooltip("The amount of frames to wait after changing direction before you start walking")]
		[Minimum(0)]
		public int DirectionDelayFrames = 3;

		[Tooltip("How fast this mover will move (in tiles per second)")]
		[Range(0.0f, 10.0f)]
		public float MoveSpeed = 5.0f;

		[Tooltip("The Renderer used as a shadow")]
		public Renderer Shadow;

		[Tooltip("The direction this mover is facing")]
		[EnumButtons]
		[SerializeField]
		private MovementDirection _direction = MovementDirection.Down;

		public UnityAction<Vector2Int, Vector2Int> OnTileChanged;
		public UnityAction<Vector2Int> OnTileEntering;
		public UnityAction<Vector2Int> OnTileExiting;
		public UnityAction<Vector2Int> OnWarp;
		public UnityAction<MovementDirection, MovementDirection> OnDirectionChanged;

		public Vector2Int TargetGridPosition { get; private set; }
		public Vector2Int PreviousGridPosition { get; private set; }
		public Vector2Int CurrentGridPosition { get; private set; }
		public MovementDirection MovementDirection => _direction;
		public bool Moving { get; private set; }
		public bool DidWarp { get; private set; }

		public bool CanInteract => !Moving;
		public float Speed => Moving ? 0.5f : 0.0f;
		public Vector2Int DirectionVector => Direction.GetVector(_direction);

		private Renderer _renderer;
		private Rigidbody2D _body;
		private Vector2 _targetPosition;
		private Interaction _interaction;

		private int _frameDelay = 0;
		private bool _skipNextMove = false;
		private bool _willWarp = false;
		private bool _changingDirection = false;
		private bool _jumping = false;
		private float _jumpTime = 0.0f;
		private Vector2 _jumpOffset = Vector2.zero;

		void Awake()
		{
			var x = Mathf.RoundToInt(transform.position.x - PositionOffset.x);
			var y = Mathf.RoundToInt(transform.position.y - PositionOffset.y);

			TargetGridPosition = new Vector2Int(x, y);

			if (_direction == MovementDirection.None)
				FaceDirection(MovementDirection.Down);

			_interaction = GetComponent<Interaction>();
			_renderer = GetComponent<Renderer>();
			_body = GetComponent<Rigidbody2D>();
			_body.bodyType = RigidbodyType2D.Kinematic;
			_body.interpolation = RigidbodyInterpolation2D.Interpolate;
		}

		void Start()
		{
			OccupyCurrentTiles();
		}

		void OnEnable()
		{
			CurrentGridPosition = TargetGridPosition;
			PreviousGridPosition = CurrentGridPosition;

			OccupyCurrentTiles();
		}

		void OnDisable()
		{
			UnoccupyCurrentTiles();
		}

		void Update()
		{
			DidWarp = _willWarp;
			_willWarp = false;
		}

		void LateUpdate()
		{
			DidWarp = false;

			var order = LayerSorting.GetSortingOrder(MovementLayer);

			_renderer.sortingOrder = order;

			if (Shadow)
				Shadow.sortingOrder = order + _shadowSortOffset;
		}

		public void SkipNextUpdate()
		{
			_skipNextMove = true;
		}

		public void FaceDirection(MovementDirection direction)
		{
			if (direction == MovementDirection.None)
				return;

			var previous = _direction;
			_direction = direction;

			if (_direction != previous)
				OnDirectionChanged?.Invoke(previous, _direction);
		}

		public void UpdateMove(float horizontal, float vertical)
		{
			var wasMoving = Moving;

			if (Moving)
			{
				var speed = Time.fixedDeltaTime * MoveSpeed;
				var previousOffset = _jumpOffset;

				if (_jumping)
				{
					_jumpTime += Time.fixedDeltaTime;
					_jumpOffset.y = GetJumpOffset(_jumpTime);
				}

				var position = Vector2.MoveTowards(_body.position - previousOffset, _targetPosition, speed);
				_body.MovePosition(position + _jumpOffset);

				if (Shadow)
					Shadow.transform.localPosition = -_jumpOffset;

				if (position == _targetPosition)
				{
					Moving = false;

					_jumping = false;
					_jumpOffset = Vector2.zero;
				}
			}

			UpdateGridPositions();

			if (!Moving && !_skipNextMove)
			{
				if (_changingDirection)
					_frameDelay++;
				else
					_frameDelay = DirectionDelayFrames;

				var direction = Direction.GetDirection(horizontal, vertical);

				if (!_changingDirection && !wasMoving)
				{
					if (direction != MovementDirection.None && direction != _direction)
						ChangeDirection(direction);
				}

				if (_frameDelay >= DirectionDelayFrames)
				{
					_changingDirection = false;

					if (direction != MovementDirection.None)
						Move(direction);
				}
			}

			_skipNextMove = false;
		}

		public void OccupyCurrentTiles()
		{
			WorldManager.Instance?.SetOccupied(CurrentGridPosition, MovementLayer);
			WorldManager.Instance?.AddInteraction(CurrentGridPosition, _interaction);
		}

		public void UnoccupyCurrentTiles()
		{
			WorldManager.Instance?.SetUnoccupied(CurrentGridPosition, MovementLayer);
			WorldManager.Instance?.RemoveInteraction(CurrentGridPosition, _interaction);

			if (Moving)
				WorldManager.Instance?.SetUnoccupied(TargetGridPosition, MovementLayer);
		}

		public void WarpToPosition(Vector2Int position, MovementDirection direction, CollisionLayer layer)
		{
			UnoccupyCurrentTiles();

			CurrentGridPosition = position;
			PreviousGridPosition = CurrentGridPosition;
			TargetGridPosition = position;
			transform.position = GetTargetPosition();
			Moving = false;

			FaceDirection(direction);

			if (layer != CollisionLayer.None && layer != CollisionLayer.All)
				MovementLayer = layer;

			_willWarp = true;

			OccupyCurrentTiles();

			OnWarp?.Invoke(position);
		}

		public void Move(MovementDirection direction)
		{
			if (direction == MovementDirection.None)
				return;

			FaceDirection(direction);

			var layer = MovementLayer;
			var moveAmount = DirectionVector;

			if (moveAmount.x != 0)
			{
				var targetTile = WorldManager.Instance.FindTile(CurrentGridPosition + moveAmount);
				if (targetTile != null && targetTile.HasStairs)
				{
					if (targetTile.LayerChange != CollisionLayer.None)
						layer = targetTile.LayerChange;

					if (moveAmount.x == targetTile.Slope)
						moveAmount.y += targetTile.Slope * moveAmount.x;
				}

				var currentTile = WorldManager.Instance.FindTile(CurrentGridPosition);
				if (currentTile != null && currentTile.HasStairs)
				{
					if (moveAmount.x != currentTile.Slope)
						moveAmount.y += currentTile.Slope * moveAmount.x;
				}
			}

			var targetPosition = CurrentGridPosition + moveAmount;

			if (GetCanMove(targetPosition, direction, layer, out bool jump))
			{
				Moving = true;
				TargetGridPosition = targetPosition;

				if (jump)
					StartJump(moveAmount);

				_targetPosition = GetTargetPosition();

				var tile = WorldManager.Instance.FindTile(TargetGridPosition);

				WorldManager.Instance.RemoveInteraction(CurrentGridPosition, _interaction);
				WorldManager.Instance.SetOccupied(TargetGridPosition, tile == null || tile.LayerChange == CollisionLayer.None ? MovementLayer : tile.LayerChange);

				OnTileExiting?.Invoke(CurrentGridPosition);
				OnTileEntering?.Invoke(TargetGridPosition);
			}
		}
	
		private void UpdateGridPositions()
		{
			PreviousGridPosition = CurrentGridPosition;
			CurrentGridPosition = Moving ? CurrentGridPosition : TargetGridPosition;

			if (PreviousGridPosition != CurrentGridPosition)
			{
				TileLeft(PreviousGridPosition);
				OnTileChanged?.Invoke(PreviousGridPosition, CurrentGridPosition);
				TileEntered(CurrentGridPosition);
			}
		}

		private void TileLeft(Vector2Int position)
		{
			WorldManager.Instance.SetUnoccupied(position, MovementLayer);
			WorldManager.Instance.RemoveInteraction(position, _interaction);
		}

		private void TileEntered(Vector2Int position)
		{
			var tile = WorldManager.Instance.FindTile(position);
			if (tile != null && tile.LayerChange != CollisionLayer.None)
				MovementLayer = tile.LayerChange;

			WorldManager.Instance.SetOccupied(position, MovementLayer);
			WorldManager.Instance.AddInteraction(position, _interaction);
		}

		private void ChangeDirection(MovementDirection direction)
		{
			if (direction == MovementDirection.None)
				return;

			_changingDirection = true;
			_frameDelay = 0;

			FaceDirection(direction);
		}

		private bool GetCanMove(Vector2Int position, MovementDirection direction, CollisionLayer layer, out bool jump)
		{
			jump = false;

			if (WorldManager.Instance.IsOccupied(position, layer))
				return false;

			var tile = WorldManager.Instance.FindTile(position);
			if (tile != null)
			{
				jump = tile.IsJumpable(direction);
				return !tile.HasCollision(layer) && !tile.IsEdge(direction) && !WorldManager.Instance.IsOccupied(tile.Position, tile.LayerChange);
			}

			return true;
		}

		private Vector2 GetTargetPosition()
		{
			var targetPosition = TargetGridPosition + PositionOffset;

			var tile = WorldManager.Instance.FindTile(TargetGridPosition);
			if (tile != null && tile.HasOffset)
				targetPosition += tile.Offset;

			return targetPosition;
		}

		private void StartJump(Vector2Int direction)
		{
			_jumping = true;
			_jumpTime = 0.0f;
			_jumpOffset = Vector2.zero;

			TargetGridPosition += direction;
		}

		private float GetJumpOffset(float time)
		{
			var distance = time * MoveSpeed;
			var x = distance - 1;
			var y = -(x * x) + 1;

			return Mathf.Max(0.0f, y);
		}
	}
}
