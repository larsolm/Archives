using System;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(MapProperties))]
	public class MapPropertiesEditor : Editor
	{
		private GUIStyle _spawnStyle = new GUIStyle();
		private GUIStyle _zoneStyle = new GUIStyle();
		private GUIStyle _collisionStyle = new GUIStyle();
		private GUIStyle _incrementStyle = new GUIStyle();
		private GUIStyle _encounterStyle = new GUIStyle();

		private void OnEnable()
		{
			_spawnStyle.normal.textColor = Color.black;
			_spawnStyle.fontStyle = FontStyle.Bold;
			_spawnStyle.alignment = TextAnchor.MiddleLeft;
			_zoneStyle.normal.textColor = Color.black;
			_zoneStyle.fontStyle = FontStyle.Bold;
			_zoneStyle.alignment = TextAnchor.UpperLeft;
			_collisionStyle.normal.textColor = Color.black;
			_collisionStyle.fontStyle = FontStyle.Bold;
			_collisionStyle.alignment = TextAnchor.UpperLeft;
			_incrementStyle.normal.textColor = Color.black;
			_incrementStyle.fontStyle = FontStyle.Bold;
			_incrementStyle.alignment = TextAnchor.LowerLeft;
			_encounterStyle.normal.textColor = Color.black;
			_encounterStyle.fontStyle = FontStyle.Bold;
			_encounterStyle.alignment = TextAnchor.LowerLeft;
		}

		private void OnSceneGUI()
		{
			var properties = target as MapProperties;

			foreach (var tile in properties.Tiles.Values)
			{
				var center = tile.Position + new Vector2(0.5f, 0.5f);
				var leftCenter = tile.Position + new Vector2(0.05f, 0.55f);
				var topLeft = tile.Position + new Vector2(0.05f, 1.0f);
				var bottomLeft = tile.Position + new Vector2(0.05f, 0.2f);

				var left = properties.GetTile(tile.Position + Vector2Int.left);
				var top = properties.GetTile(tile.Position + Vector2Int.up);
				var bottom = properties.GetTile(tile.Position + Vector2Int.down);

				if (tile.HasSpawnPoint)
				{
					DrawTextHandle(null, null, tile.SpawnPoint.Name, leftCenter, _spawnStyle, null);

					if (tile.SpawnPoint.Direction != Vector2.zero)
						Handles.ArrowHandleCap(0, center, Quaternion.LookRotation(tile.SpawnPoint.Direction), 0.5f, EventType.Repaint);
				}

				if (tile.HasZoneTrigger && tile.Zone.TargetZone != null) DrawTextHandle(top, left, tile.Zone.TargetZone.name, topLeft, _zoneStyle, tile.IsSameZoneAs);
				if (tile.CollisionLayer != CollisionLayer.None) DrawTextHandle(top, left, tile.CollisionLayer.ToString(), topLeft, _collisionStyle, tile.IsSameCollisionLayer);
				if (tile.CollisionLayerIncrement != CollisionLayer.None) DrawTextHandle(bottom, left, tile.CollisionLayerIncrement.ToString(), bottomLeft, _incrementStyle, tile.IsSameCollisionLayerIncrement);
				if (tile.HasEncounter && tile.Encounter != null) DrawTextHandle(top, left, tile.Encounter.name, topLeft, _encounterStyle, tile.IsSameEncounterAs);
			}
		}

		private void DrawTextHandle(TileInfo first, TileInfo second, string text, Vector2 position, GUIStyle style, Func<TileInfo, bool> func)
		{
			if (!string.IsNullOrEmpty(text))
			{

				if (func == null || (!func.Invoke(first) && !func.Invoke(second)))
					Handles.Label(position, text, style);
			}
		}
	}
}
