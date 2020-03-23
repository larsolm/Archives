using System;
using UnityEngine;
using UnityEngine.Events;

namespace PiRhoSoft.MonsterMaker
{
	[DisallowMultipleComponent]
	public abstract class Mover : MonoBehaviour
	{
		public readonly Vector2 PositionOffset = new Vector2(0.5f, 0.5f);

		[Tooltip("Which Collision Layer this mover will start on.")] public CollisionLayer CollisionLayer = CollisionLayer.One;
		[Tooltip("How fast this mover will move (in cells per second).")] [Range(0.0f, 10.0f)] public float MoveSpeed = 5.0f;
		[NonSerialized] public bool DelayForFrame = false;

		public UnityAction<Vector2Int, Vector2Int> OnTileChanged;

		public Vector2Int PreviousGridPosition { get; protected set; }
		public Vector2Int CurrentGridPosition { get; protected set; }
		public ZoneData CurrentZone { get; private set; }
		public bool DidWarp { get; private set; }

		public virtual bool CanInteract { get { return true; } }
		public abstract float Speed { get; }
		public abstract Vector2 MoveDirection { get; }

		protected abstract Vector2Int GridPosition { get; }
		
		public abstract void UpdateMove(float horizontal, float vertical);
		public abstract void OccupyCurrentTiles();
		public abstract void UnoccupyCurrentTiles();

		public bool CanCollide(Collider2D collider)
		{
			var collisionLayer = collider.GetComponent<CollisionLayerData>();
			return !collisionLayer || ((collisionLayer.CollisionLayer & CollisionLayer) != 0);
		}

		public void SetCurrentZone(ZoneData zone)
		{
			CurrentZone = zone;
		}

		public virtual void WarpToPosition(Vector2Int position, Vector2 direction, CollisionLayer layer)
		{
			UnoccupyCurrentTiles();

			transform.position = position + PositionOffset;
			CurrentGridPosition = position;
			PreviousGridPosition = CurrentGridPosition;
			CollisionLayer = layer;
			_willWarp = true;

			OccupyCurrentTiles();
		}

		protected void UpdateGridPositions()
		{
			PreviousGridPosition = CurrentGridPosition;
			CurrentGridPosition = GridPosition;

			if (PreviousGridPosition != CurrentGridPosition)
			{
				TileLeft(PreviousGridPosition);

				if (OnTileChanged != null)
					OnTileChanged(PreviousGridPosition, CurrentGridPosition);
				
				TileEntered(CurrentGridPosition);
			}
		}
		
		protected virtual void Start()
		{
			OccupyCurrentTiles();
		}

		protected virtual void OnEnable()
		{
			CurrentGridPosition = GridPosition;
			PreviousGridPosition = CurrentGridPosition;

			if (WorldManager.Instance != null)
				SetCurrentZone(WorldManager.Instance.GetZone(gameObject));

			if (CurrentZone != null && CurrentZone.Tilemap != null)
				OccupyCurrentTiles();
		}

		protected virtual void OnDisable()
		{
			if (CurrentZone != null && CurrentZone.Tilemap != null)
				UnoccupyCurrentTiles();

			SetCurrentZone(null);
		}

		protected virtual void TileLeft(Vector2Int position)
		{
		}

		protected virtual void TileEntered(Vector2Int position)
		{
			if (CurrentZone != null)
			{
				var tile = CurrentZone.Tilemap.GetTile(position);

				if (tile != null && tile.CollisionLayerIncrement != CollisionLayer.None)
					CollisionLayer = tile.CollisionLayerIncrement;
			}
		}

		private bool _willWarp = false;

		private void Update()
		{
			// TODO: blech
			DidWarp = _willWarp;
			_willWarp = false;
		}

		private void LateUpdate()
		{
			DidWarp = false;
		}
	}
}
