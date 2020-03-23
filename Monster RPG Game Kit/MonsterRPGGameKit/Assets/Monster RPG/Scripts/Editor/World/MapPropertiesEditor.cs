using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	[CustomEditor(typeof(MapProperties))]
	public class MapPropertiesEditor : Editor
	{
		private static readonly TextButton _createPathfindingButton = new TextButton("Create Pathfinding", "Create a pathfinding component on this map object");
		private static readonly TextButton _refreshTilesButton = new TextButton("Refresh Tiles", "Parse empty tiles and regenerate the pathfinding nodes on this map object");
		private static readonly TextButton _calculateButton = new TextButton("Calculate", "Calculate the the bounds based on the map tiles and any tilemap in this scene");
		private static readonly Label _clampLeftBoundsContent = new Label(typeof(MapProperties), nameof(MapProperties.ClampLeftBounds));
		private static readonly Label _clampRightBoundsContent = new Label(typeof(MapProperties), nameof(MapProperties.ClampRightBounds));
		private static readonly Label _clampTopBoundsContent = new Label(typeof(MapProperties), nameof(MapProperties.ClampTopBounds));
		private static readonly Label _clampBottomBoundsContent = new Label(typeof(MapProperties), nameof(MapProperties.ClampBottomBounds));

		private MapProperties _mapProperties;

		void OnEnable()
		{
			_mapProperties = target as MapProperties;
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(_mapProperties, false))
			{
				DrawBounds(_clampLeftBoundsContent.Content, ref _mapProperties.ClampLeftBounds, ref _mapProperties.LeftBounds, _mapProperties.CalculateLeft);
				DrawBounds(_clampRightBoundsContent.Content, ref _mapProperties.ClampRightBounds, ref _mapProperties.RightBounds, _mapProperties.CalculateRight);
				DrawBounds(_clampTopBoundsContent.Content, ref _mapProperties.ClampTopBounds, ref _mapProperties.TopBounds, _mapProperties.CalculateTop);
				DrawBounds(_clampBottomBoundsContent.Content, ref _mapProperties.ClampBottomBounds, ref _mapProperties.BottomBounds, _mapProperties.CalculateBottom);
			}

			if (GUILayout.Button(_refreshTilesButton.Content))
				_mapProperties.RefreshTiles();

			var pathfinding = _mapProperties.GetComponent<Pathfinding>();
			if (!pathfinding)
			{
				if (GUILayout.Button(_createPathfindingButton.Content))
				{
					var path = Undo.AddComponent<Pathfinding>(_mapProperties.gameObject);
					path.RegenerateNodes();
				}
			}
		}

		private void DrawBounds(GUIContent content, ref bool clamp, ref float bounds, Func<float> calculate)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				clamp = EditorGUILayout.Toggle(content, clamp);
				if (clamp)
				{
					bounds = EditorGUILayout.FloatField(bounds);
					if (GUILayout.Button(_calculateButton.Content))
						bounds = calculate();
				}
			}
		}

		private void OnSceneGUI()
		{
			if (_mapProperties.ClampBounds)
			{
				var bounds = _mapProperties.GetBounds();

				var xMin = _mapProperties.ClampLeftBounds ? _mapProperties.LeftBounds : bounds.xMin;
				var yMin = _mapProperties.ClampBottomBounds ? _mapProperties.BottomBounds : bounds.yMin;
				var width = (_mapProperties.ClampRightBounds ? _mapProperties.RightBounds : bounds.xMax) - xMin;
				var height = (_mapProperties.ClampTopBounds ? _mapProperties.TopBounds : bounds.yMax) - yMin;

				if (_mapProperties.ClampLeftBounds)
					Handles.DrawSolidRectangleWithOutline(new Rect(_mapProperties.LeftBounds - 0.5f, yMin, 0.5f, height), Color.black, Color.black);

				if (_mapProperties.ClampRightBounds)
					Handles.DrawSolidRectangleWithOutline(new Rect(_mapProperties.RightBounds, yMin, 0.5f, height), Color.black, Color.black);

				if (_mapProperties.ClampTopBounds)
					Handles.DrawSolidRectangleWithOutline(new Rect(xMin, _mapProperties.TopBounds, width, 0.5f), Color.black, Color.black);

				if (_mapProperties.ClampBottomBounds)
					Handles.DrawSolidRectangleWithOutline(new Rect(xMin, _mapProperties.BottomBounds - 0.5f, width, 0.5f), Color.black, Color.black);
			}

			var camera = Camera.current;
			var screenRect = new Rect(camera.ScreenToWorldPoint(Vector2.zero) - Vector3.one, new Vector2(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2) + Vector2.one);

			if (camera.orthographicSize > 10)
				return;

			foreach (var tile in _mapProperties.Tiles.Values)
			{
				if (!screenRect.Contains(tile.Position))
					continue;

				var center = tile.Position + new Vector2(0.5f, 0.5f);
				var leftCenter = tile.Position + new Vector2(0.05f, 0.55f);
				var topLeft = tile.Position + new Vector2(0.05f, 1.0f);
				var bottomLeft = tile.Position + new Vector2(0.05f, 0.2f);

				var left = _mapProperties.GetTile(tile.Position + Vector2Int.left);
				var top = _mapProperties.GetTile(tile.Position + Vector2Int.up);
				var bottom = _mapProperties.GetTile(tile.Position + Vector2Int.down);

				if (tile.HasSpawnPoint && !string.IsNullOrEmpty(tile.SpawnPoint.Name))
				{
					if (tile.SpawnPoint.Direction != MovementDirection.None)
						HandleHelper.DrawArrow(center, Direction.GetVector(tile.SpawnPoint.Direction), 0.5f, Color.white);

					DrawTextHandle(null, null, tile.SpawnPoint.Name, leftCenter, TextAnchor.MiddleLeft, Color.white, null);
				}

				if (tile.HasStairs)
				{
					HandleHelper.DrawArrow(tile.Position + new Vector2(0.0f, 0.5f - (tile.Slope * 0.5f)), new Vector2(1.0f, tile.Slope), 1.0f, Color.white);

					DrawTextHandle(top, left, "Slope: " + tile.Slope.ToString(), leftCenter, TextAnchor.MiddleLeft, Color.white, tile.IsSameStairsAs);
				}

				if (tile.HasEdge)
				{
					if (tile.EdgeDirection != MovementDirection.None)
						HandleHelper.DrawArrow(center, Direction.GetVector(tile.EdgeDirection), 0.5f, Color.white);

					DrawTextHandle(top, left, "Edge: " + tile.EdgeDirection.ToString(), leftCenter, TextAnchor.MiddleLeft, Color.white, tile.IsSameEdgeAs);
				}

				if (tile.HasOffset) DrawTextHandle(bottom, left, "Offset: " + tile.Offset.ToString(), bottomLeft, TextAnchor.LowerLeft, Color.white, tile.IsSameOffsetAs);
				if (tile.HasZoneTrigger && tile.Zone.TargetZone != null) DrawTextHandle(top, left, "Target Zone:\n" + tile.Zone.TargetZone.name, topLeft, TextAnchor.UpperLeft, Color.white, tile.IsSameZoneAs);
				if (tile.CollisionLayer != CollisionLayer.None) DrawTextHandle(top, left, "Collision:\n" + tile.CollisionLayer.ToString(), topLeft, TextAnchor.UpperLeft, Color.white, tile.IsSameCollisionLayer);
				if (tile.LayerChange != CollisionLayer.None) DrawTextHandle(bottom, left, "Layer: " + tile.LayerChange.ToString(), bottomLeft, TextAnchor.LowerLeft, Color.white, tile.IsSameCollisionLayerIncrement);
				if (tile.HasEncounter && tile.Encounter != null) DrawTextHandle(top, left, "Encounter:\n " + tile.Encounter.name, topLeft, TextAnchor.UpperLeft, Color.white, tile.IsSameEncounterAs);
			}
		}

		private void DrawTextHandle(TileInfo first, TileInfo second, string text, Vector2 position, TextAnchor alignment, Color color, Func<TileInfo, bool> func)
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (func == null || (!func.Invoke(first) && !func.Invoke(second)))
					HandleHelper.DrawText(position, text, alignment, color);
			}
		}

		[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
		private static void OnDrawGizmos(MapProperties properties, GizmoType gizmoType)
		{
			var camera = Camera.current;
			var screenRect = new Rect(camera.ScreenToWorldPoint(Vector2.zero) - Vector3.one, new Vector2(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2) + Vector2.one);

			foreach (var tile in properties.Tiles.Values)
			{
				if (!screenRect.Contains(tile.Position))
					continue;

				var center = tile.Position + new Vector2(0.5f, 0.5f);
				var topLeft = tile.Position + Vector2.up;
				var topRight = tile.Position + Vector2.one;
				var bottomLeft = tile.Position + Vector2.zero;
				var bottomRight = tile.Position + Vector2.right;

				var left = properties.GetTile(tile.Position + Vector2Int.left);
				var right = properties.GetTile(tile.Position + Vector2Int.right);
				var top = properties.GetTile(tile.Position + Vector2Int.up);
				var bottom = properties.GetTile(tile.Position + Vector2Int.down);

				if (tile.CollisionLayer != CollisionLayer.None) DrawDebugBoxes(left, right, top, bottom, new Color(1.0f, 0.0f, 0.0f, 0.5f), Color.red, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameCollisionLayer);
				if (tile.LayerChange != CollisionLayer.None) DrawDebugBoxes(left, right, top, bottom, new Color(1.0f, 0.0f, 1.0f, 0.5f), Color.magenta, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameCollisionLayerIncrement);
				if (tile.HasSpawnPoint && !string.IsNullOrEmpty(tile.SpawnPoint.Name)) DrawDebugBoxes(left, right, top, bottom, new Color(1.0f, 1.0f, 0.0f, 0.5f), Color.yellow, center, topLeft, topRight, bottomLeft, bottomRight, null);
				if (tile.HasZoneTrigger) DrawDebugBoxes(left, right, top, bottom, new Color(0.0f, 0.0f, 1.0f, 0.5f), Color.blue, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameZoneAs);
				if (tile.HasEncounter) DrawDebugBoxes(left, right, top, bottom, new Color(0.0f, 1.0f, 0.0f, 0.5f), Color.green, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameEncounterAs);
				if (tile.HasInstructions) DrawDebugBoxes(left, right, top, bottom, new Color(0.0f, 1.0f, 1.0f, 0.5f), Color.cyan, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameInstructionAs);
				if (tile.HasStairs) DrawDebugBoxes(left, right, top, bottom, new Color(0.0f, 0.0f, 0.0f, 0.5f), Color.black, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameStairsAs);
				if (tile.HasEdge) DrawDebugBoxes(left, right, top, bottom, new Color(0.0f, 0.0f, 0.0f, 0.5f), Color.black, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameEdgeAs);
				if (tile.HasOffset) DrawDebugBoxes(left, right, top, bottom, new Color(0.0f, 0.0f, 0.0f, 0.5f), Color.black, center, topLeft, topRight, bottomLeft, bottomRight, tile.IsSameOffsetAs);
			}
		}

		private static void DrawDebugBoxes(TileInfo left, TileInfo right, TileInfo top, TileInfo bottom, Color fill, Color outline, Vector2 center, Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight, Func<TileInfo, bool> func)
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
