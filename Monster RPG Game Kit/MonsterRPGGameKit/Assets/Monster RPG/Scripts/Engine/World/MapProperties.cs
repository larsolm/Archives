using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterRpgEngine
{
	[DisallowMultipleComponent]
	[HelpURL(MonsterRpg.DocumentationUrl + "map-properties")]
	[AddComponentMenu("PiRho Soft/World/Map Properties")]
	public class MapProperties : MonoBehaviour
	{
		[Tooltip("Whether or not the camera should clamp to the left point of this Map")] public bool ClampLeftBounds;
		[Tooltip("Whether or not the camera should clamp to the right point of this Map")] public bool ClampRightBounds;
		[Tooltip("Whether or not the camera should clamp to the bottom point of this Map")] public bool ClampBottomBounds;
		[Tooltip("Whether or not the camera should clamp to the top point of this Zone")] public bool ClampTopBounds;

		[Tooltip("The left point at which the camera will clamp")] public float LeftBounds;
		[Tooltip("The right point at which the camera will clamp")] public float RightBounds;
		[Tooltip("The bottom point at which the camera will clamp")] public float BottomBounds;
		[Tooltip("The top point at which the camera will clamp")] public float TopBounds;

		[SerializeField] private TileDictionary _tiles = new TileDictionary();

		public bool ClampBounds => ClampLeftBounds || ClampRightBounds || ClampBottomBounds || ClampTopBounds;
		public TileDictionary Tiles => _tiles;

		void Awake()
		{
			var objects = gameObject.scene.GetRootGameObjects();

			foreach (var obj in objects)
				obj.SetActive(false);
		}

		public TileInfo AddOrGetTile(Vector2Int position)
		{
			return GetTile(position) ?? AddTile(position);
		}

		public TileInfo GetTile(Vector2Int position)
		{
			_tiles.TryGetValue(position, out TileInfo tile);
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

		public void RefreshTiles()
		{
			var toRemove = new List<Vector2Int>();

			foreach (var tile in _tiles.Values)
			{
				if (tile.IsEmpty())
					toRemove.Add(tile.Position);
			}

			foreach (var position in toRemove)
				_tiles.Remove(position);
		}

		public RectInt GetBounds()
		{
			var tilemaps = GetComponentsInChildren<Tilemap>();
			var left = int.MaxValue;
			var right = int.MinValue;
			var top = int.MinValue;
			var bottom = int.MaxValue;

			foreach (var tile in _tiles.Values)
			{
				left = Math.Min(left, tile.Position.x);
				right = Math.Max(right, tile.Position.x);
				top = Math.Max(top, tile.Position.y);
				bottom = Math.Min(bottom, tile.Position.y);
			}

			foreach (var tilemap in tilemaps)
			{
				tilemap.CompressBounds();
				if (tilemap.GetUsedTilesCount() == 0)
					continue;

				var bounds = tilemap.cellBounds;
				var position = tilemap.transform.position;

				left = Math.Min(left, Mathf.RoundToInt(bounds.xMin + position.x));
				right = Math.Max(right, Mathf.RoundToInt(bounds.xMax + position.x));
				top = Math.Max(top, Mathf.RoundToInt(bounds.yMax + position.y));
				bottom = Math.Min(bottom, Mathf.RoundToInt(bounds.yMin + position.y));
			}

			return new RectInt(left, bottom, right - left, top - bottom);
		}

		public float CalculateLeft()
		{
			var bounds = GetBounds();
			return bounds.xMin;
		}

		public float CalculateRight()
		{
			var bounds = GetBounds();
			return bounds.xMax;
		}

		public float CalculateTop()
		{
			var bounds = GetBounds();
			return bounds.yMax;
		}

		public float CalculateBottom()
		{
			var bounds = GetBounds();
			return bounds.yMin;
		}
	}
}