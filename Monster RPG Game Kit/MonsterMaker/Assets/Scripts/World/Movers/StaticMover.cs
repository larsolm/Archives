using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[RequireComponent(typeof(Collider2D))]
	[AddComponentMenu("Monster Maker/Movers/Static Mover")]
	public class StaticMover : Mover
	{
		public override float Speed { get { return 0.0f; } }
		public override Vector2 MoveDirection { get { return Vector2.zero; } }
		protected override Vector2Int GridPosition { get { return Vector2Int.FloorToInt(transform.position); } }

		private RectInt _occupiedTiles;

		public override void UpdateMove(float horizontal, float vertical)
		{
		}

		public override void OccupyCurrentTiles()
		{
			if (CurrentZone != null)
			{
				foreach (var position in _occupiedTiles.allPositionsWithin)
					CurrentZone.Tilemap.SetOccupied(position, CollisionLayer);
			}
		}

		public override void UnoccupyCurrentTiles()
		{
			if (CurrentZone != null)
			{
				foreach (var position in _occupiedTiles.allPositionsWithin)
					CurrentZone.Tilemap.SetUnoccupied(position, CollisionLayer);
			}
		}

		protected virtual void Awake()
		{
			var collider = GetComponent<Collider2D>();
			var bounds = collider.bounds;
			_occupiedTiles = new RectInt(Vector2Int.FloorToInt(bounds.min), Vector2Int.FloorToInt(bounds.size));
		}
	}
}
