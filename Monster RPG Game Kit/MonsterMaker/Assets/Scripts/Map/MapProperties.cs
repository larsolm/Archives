using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/World/Tilemap Properties")]
	public class MapProperties : MonoBehaviour
	{
		[Serializable]
		public class TileDictionary : SerializedDictionary<Vector2Int, TileInfo> { }

		[HideInInspector] [SerializeField] private TileDictionary _tiles = new TileDictionary();
		[HideInInspector] [SerializeField] private Tilemap[] _collisionLayers = new Tilemap[CollisionLayerData.LayerCount];
		[ReadOnly] [SerializeField] private RectInt _bounds;

		public TileDictionary Tiles { get { return _tiles; } }
		public RectInt Bounds { get { return _bounds; } }

		private RectInt _infoBounds;

		public TileInfo AddOrGetTile(Vector2Int position)
		{
			return GetTile(position) ?? AddTile(position);
		}

		public TileInfo GetTile(Vector2Int position)
		{
			TileInfo tile;
			if (!Tiles.TryGetValue(position, out tile))
				return null;

			return tile;
		}

		public TileInfo AddTile(Vector2Int position)
		{
			var tile = new TileInfo
			{
				Position = position
			};

			_tiles.Add(position, tile);

			return tile;
		}

		public SpawnPoint GetValidSpawn()
		{
			var width = _bounds.width;
			var height = _bounds.height;
			var x = 0;
			var y = 0;
			var dx = 0;
			var dy = -1;

			var max = Math.Max(_bounds.width, _bounds.height);
			var count = max * max;
			for (int i = 0; i < count; i++)
			{
				if ((-width / 2 <= x) && (x <= width / 2) && (-height / 2 <= y) && (y <= height / 2))
				{
					var tile = GetTile(new Vector2Int(x, y));
					if (tile == null || !tile.IsOccupiedOrHasCollision(CollisionLayer.All))
						return new SpawnPoint { Position = tile.Position, Direction = Vector2Int.down, Layer = tile == null || tile.CollisionLayerIncrement == CollisionLayer.None ? CollisionLayer.One : tile.CollisionLayerIncrement };
				}

				if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y)))
				{
					max = dx;
					dx = -dy;
					dy = max;
				}

				x += dx;
				y += dy;
			}

			return new SpawnPoint { Direction = Vector2Int.down, Layer = CollisionLayer.One };
		}

		public void SetOccupied(Vector2Int position, CollisionLayer layer)
		{
			if (!_bounds.Contains(position))
				return;

			var tile = AddOrGetTile(position);
			tile.OccupiedLayer |= (tile.CollisionLayerIncrement == CollisionLayer.None ? layer : tile.CollisionLayerIncrement);
		}

		public void SetUnoccupied(Vector2Int position, CollisionLayer layer)
		{
			var tile = GetTile(position);
			if (tile == null)
				return;

			tile.OccupiedLayer &= (tile.CollisionLayerIncrement == CollisionLayer.None ? ~layer : ~tile.CollisionLayerIncrement);

			if (tile.IsEmpty())
				_tiles.Remove(position);
		}

		public Tilemap AddOrGetCollisionLayer(int layer)
		{
			return GetCollisionLayer(layer) ?? AddCollisionLayer(layer);
		}

		public Tilemap GetCollisionLayer(int layer)
		{
			if (_collisionLayers.Length < CollisionLayerData.LayerCount)
				Array.Resize(ref _collisionLayers, CollisionLayerData.LayerCount);
	
			return _collisionLayers[layer];
		}

		public Tilemap AddCollisionLayer(int layerIndex)
		{
			var newObject = new GameObject("CollisionLayer" + (layerIndex + 1).ToString());
			newObject.transform.parent = transform;

			_collisionLayers[layerIndex] = newObject.AddComponent<Tilemap>();

			var collision = newObject.AddComponent<CollisionLayerData>();
			var collider = newObject.AddComponent<TilemapCollider2D>();
			var body = newObject.AddComponent<Rigidbody2D>();
			
			newObject.AddComponent<CompositeCollider2D>();
			collision.CollisionLayer = (CollisionLayer)MathHelper.IntExponent(2, layerIndex);
			body.bodyType = RigidbodyType2D.Static;
			collider.usedByComposite = true;

			return _collisionLayers[layerIndex];
		}

		public void CheckForCollisionRemoval(Tilemap tilemap, int layerIndex)
		{
			var count = tilemap.GetUsedTilesCount();
			if (count == 0)
			{
				DestroyImmediate(tilemap.gameObject);
				_collisionLayers[layerIndex] = null;
			}
		}

		public void AddConnections(List<int> connections)
		{
			foreach (var tile in _tiles.Values)
			{
				if (tile.HasZoneTrigger && tile.Zone.TargetZone)
				{
					if (!connections.Contains(tile.Zone.TargetZone.Scene.Index))
						connections.Add(tile.Zone.TargetZone.Scene.Index);
				}
			}
		}

		public void UpdateBounds()
		{
			var tilemaps = GetComponentsInChildren<Tilemap>();
			var left = int.MaxValue;
			var right = int.MinValue;
			var bottom = int.MaxValue;
			var top = int.MinValue;

			var toRemove = new List<Vector2Int>();

			foreach (var tile in _tiles.Values)
			{
				if (tile.IsEmpty())
					toRemove.Add(tile.Position);
			}

			foreach (var position in toRemove)
				_tiles.Remove(position);

			foreach (var tile in _tiles.Values)
			{
				left = Math.Min(left, tile.Position.x);
				right = Math.Max(right, tile.Position.x);
				bottom = Math.Min(bottom, tile.Position.y);
				top = Math.Max(top, tile.Position.y);
			}

			foreach (var tilemap in tilemaps)
			{
				tilemap.CompressBounds();

				var bounds = tilemap.cellBounds;
				var position = tilemap.transform.position;

				left = Math.Min(left, Mathf.RoundToInt(bounds.xMin + position.x));
				right = Math.Max(right, Mathf.RoundToInt(bounds.xMax + position.x));
				bottom = Math.Min(bottom, Mathf.RoundToInt(bounds.yMin + position.y));
				top = Math.Max(top, Mathf.RoundToInt(bounds.yMax + position.y));
			}

			_bounds = new RectInt(left, bottom, right - left, top - bottom);

			var pathfinding = GetComponent<Pathfinding>();
			if (pathfinding)
				pathfinding.RegenerateNodes();
		}

		public void AddSpawnPoints(Dictionary<string, SpawnPoint> spawnPoints)
		{
			foreach (var tile in _tiles.Values)
			{
				if (tile.HasSpawnPoint && !string.IsNullOrEmpty(tile.SpawnPoint.Name))
				{
					if (!spawnPoints.ContainsKey(tile.SpawnPoint.Name))
						spawnPoints.Add(tile.SpawnPoint.Name, tile.SpawnPoint);
				}
			}
		}

		private void Awake()
		{
			WorldManager.Instance.GetZone(gameObject).SetTilemap(this);
		}

		private void OnDestroy()
		{
			if (WorldManager.Instance != null)
				WorldManager.Instance.GetZone(gameObject).SetTilemap(null);
		}

		private void OnDrawGizmos()
		{
			foreach (var tile in _tiles.Values)
			{
				var center = tile.Position + new Vector2(0.5f, 0.5f);
				var topLeft = tile.Position + Vector2.up;
				var topRight = tile.Position + Vector2.one;
				var bottomLeft = (Vector2)tile.Position;
				var bottomRight = tile.Position + Vector2.right;
				
				var left = GetTile(tile.Position + Vector2Int.left);
				var right = GetTile(tile.Position + Vector2Int.right);
				var top = GetTile(tile.Position + Vector2Int.up);
				var bottom = GetTile(tile.Position + Vector2Int.down);
				
				if (tile.CollisionLayer != CollisionLayer.None) DrawDebugBoxes(left, right, top, bottom, new Color(1.0f, 0.0f, 0.0f, 0.5f),  Color.red, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameCollisionLayer);
				if (tile.OccupiedLayer != CollisionLayer.None) DrawDebugBoxes(left, right, top, bottom, new Color(1.0f, 1.0f, 1.0f, 0.5f), Color.white, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameOccupiedLayer);
				if (tile.CollisionLayerIncrement != CollisionLayer.None) DrawDebugBoxes(left, right, top, bottom, new Color(1.0f, 0.0f, 1.0f, 0.5f), Color.magenta, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameCollisionLayerIncrement);
				if (tile.HasSpawnPoint && !string.IsNullOrEmpty(tile.SpawnPoint.Name)) DrawDebugBoxes(left, right, top, bottom, new Color(1.0f, 1.0f, 0.0f, 0.5f), Color.yellow, center, topLeft, topRight, bottomLeft, bottomRight, null);
				if (tile.HasZoneTrigger) DrawDebugBoxes(left, right, top, bottom, new Color(0.0f, 0.0f, 1.0f, 0.5f), Color.blue, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameZoneAs);
				if (tile.HasEncounter) DrawDebugBoxes(left, right, top, bottom, new Color(0.0f, 1.0f, 0.0f, 0.5f), Color.green, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameEncounterAs);
				if (tile.HasInstructions && tile.Instructions != null) DrawDebugBoxes(left, right, top, bottom, new Color(0.0f, 1.0f, 1.0f, 0.5f), Color.cyan, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameInstructionAs);
			}
		}

		private void DrawDebugBoxes(TileInfo left, TileInfo right, TileInfo top, TileInfo bottom, Color fill, Color outline, Vector2 center, Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight, Func<TileInfo, bool> func)
		{
			Gizmos.color = fill;
			Gizmos.DrawCube(center, Vector3.one);
			Gizmos.color = outline;

			if (func == null || !func.Invoke(left)) Gizmos.DrawLine(bottomLeft, topLeft);
			if (func == null || !func.Invoke(right)) Gizmos.DrawLine(topRight, bottomRight);
			if (func == null || !func.Invoke(top)) Gizmos.DrawLine(topLeft, topRight);
			if (func == null || !func.Invoke(bottom)) Gizmos.DrawLine(bottomRight, bottomLeft);
		}
	}
}
