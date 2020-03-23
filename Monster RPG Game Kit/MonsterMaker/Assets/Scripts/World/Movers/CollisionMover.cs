using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[RequireComponent(typeof(Rigidbody2D))]
	public abstract class CollisionMover : Mover
	{	
		public override float Speed { get { return _velocity.sqrMagnitude; } }
		protected override Vector2Int GridPosition { get { return Vector2Int.FloorToInt(_body.position); } }

		private const int _maxCollisionIterations = 4;
		private const float _collisionCorrection = 0.05f;
		
		protected Rigidbody2D _body;
		protected ContactFilter2D _contactFilter;
		protected RaycastHit2D[] _collisions = new RaycastHit2D[8];
		protected Vector2 _velocity = Vector2.zero;

		private RectInt _previousOccupiedTiles;
		private RectInt _currentOccupiedTiles;

		public override void UnoccupyCurrentTiles()
		{
			_currentOccupiedTiles = GetCurrentOccupiedTiles();
			_previousOccupiedTiles = _currentOccupiedTiles;

			if (CurrentZone != null)
			{
				foreach (var position in _currentOccupiedTiles.allPositionsWithin)
					CurrentZone.Tilemap.SetOccupied(position, CollisionLayer);
			}
		}

		public override void OccupyCurrentTiles()
		{
			_currentOccupiedTiles = GetCurrentOccupiedTiles();
			_previousOccupiedTiles = _currentOccupiedTiles;

			if (CurrentZone != null)
			{
				foreach (var position in _currentOccupiedTiles.allPositionsWithin)
					CurrentZone.Tilemap.SetOccupied(position, CollisionLayer);
			}
		}

		protected virtual void Awake()
		{
			_body = GetComponent<Rigidbody2D>();
			_body.bodyType = RigidbodyType2D.Kinematic;
			_body.interpolation = RigidbodyInterpolation2D.Interpolate;

			_contactFilter.useTriggers = false;
			_contactFilter.useLayerMask = true;
			_contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
		}

		protected void Move()
		{
			var elapsed = Time.fixedDeltaTime;

			for (var iterations = 0; iterations < _maxCollisionIterations && _velocity.sqrMagnitude > 0.0f && elapsed > 0.0f; iterations++)
			{
				if (!IterateCollisions(ref elapsed))
					break;
			}

			_body.MovePosition(_body.position + (_velocity * elapsed));

			_previousOccupiedTiles = _currentOccupiedTiles;
			_currentOccupiedTiles = GetCurrentOccupiedTiles();

			UpdateUnoccupiedTiles();
			UpdateGridPositions();
			UpdateOccupiedTiles();
		}

		private bool IterateCollisions(ref float elapsed)
		{
			var moveDistance = _velocity.magnitude * elapsed;
			var count = _body.Cast(_velocity, _contactFilter, _collisions, moveDistance + _collisionCorrection);
			var smallestTime = float.MaxValue;
			var smallest = -1;

			for (var i = 0; i < count; i++)
			{
				var collision = _collisions[i];
				if (CanCollide(collision.collider))
				{
					var dot = Vector2.Dot(collision.normal, _velocity);
					if (dot < 0.0f)
					{
						var time = (collision.distance - _collisionCorrection) / moveDistance;
						if (time < smallestTime)
						{
							smallestTime = time;
							smallest = i;
						}
					}
				}
			}

			if (smallest >= 0)
			{
				_body.MovePosition(_body.position + (_velocity * smallestTime * elapsed));

				elapsed *= 1 - smallestTime;

				var normal = _collisions[smallest].normal;
				var projection = Vector2.Dot(_velocity, normal);

				_velocity -= normal * projection;

				return true;
			}

			return false;
		}

		private RectInt GetCurrentOccupiedTiles()
		{
			return new RectInt(Vector2Int.FloorToInt(_body.position - PositionOffset), Vector2Int.FloorToInt(_body.position + PositionOffset));
		}

		private void UpdateUnoccupiedTiles()
		{
			if (CurrentZone != null)
			{
				foreach (var position in _previousOccupiedTiles.allPositionsWithin)
				{
					if (!_currentOccupiedTiles.Contains(position))
						CurrentZone.Tilemap.SetUnoccupied(position, CollisionLayer);
				}
			}
		}

		private void UpdateOccupiedTiles()
		{
			if (CurrentZone != null)
			{
				foreach (var position in _currentOccupiedTiles.allPositionsWithin)
				{
					if (!_previousOccupiedTiles.Contains(position))
						CurrentZone.Tilemap.SetOccupied(position, CollisionLayer);
				}
			}
		}
	}
}
