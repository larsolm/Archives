using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using PiRhoSoft.UtilityEngine;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	[CustomEditor(typeof(PathController))]
	class PathControllerEditor : Editor
	{
		private static readonly GUIContent _createPathfindingContent = new GUIContent("Create\nNow", "Create a pathfinding component on this scene's map object");

		private PathController _pathController;
		private SerializedProperty _begin;
		private SerializedProperty _path;

		void OnEnable()
		{
			_pathController = target as PathController;

			_begin = serializedObject.FindProperty(nameof(PathController.BeginOnAwake));
			_path = serializedObject.FindProperty(nameof(PathController.Path));
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_begin);
				EditorGUILayout.PropertyField(_path);
			}

			if (_pathController.Path.UsePathfinding)
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					var pathfinding = ComponentHelper.GetComponentInScene<Pathfinding>(_pathController.gameObject.scene.buildIndex, false);
					if (pathfinding == null)
					{
						EditorGUILayout.HelpBox("In order to use pathfinding this scene's map object must have a pathfinding component attached", MessageType.Warning, true);

						if (GUILayout.Button(_createPathfindingContent, GUILayout.Height(39)))
						{
							var properties = ComponentHelper.GetComponentInScene<MapProperties>(_pathController.gameObject.scene.buildIndex, false);
							if (properties)
							{
								var path = Undo.AddComponent<Pathfinding>(properties.gameObject);
								path.RegenerateNodes();
							}
						}
					}
				}
			}
		}

		private void OnSceneGUI()
		{
			if (_pathController.Path.Nodes.Length > 0)
			{
				var lineColor = Color.cyan;
				var fillColor = new Color(0.0f, 1.0f, 1.0f, 0.2f);
				var mover = _pathController.GetComponent<Mover>();
				var pathfinding = ComponentHelper.GetComponentInScene<Pathfinding>(_pathController.gameObject.scene.buildIndex, false);
				var transformPosition = _pathController.Path.UseAbsolutePositioning ? Vector2.zero : (Vector2)_pathController.transform.position;

				if (_pathController.Path.Nodes.Length > 2 && _pathController.Path.Type == Path.PathType.Loop)
				{
					var first = _pathController.Path.Nodes[0];
					var last = _pathController.Path.Nodes[_pathController.Path.Nodes.Length - 1];

					var firstPosition = GetPosition(first.Position, _pathController.transform.position, Mover.PositionOffset);
					var lastPosition = GetPosition(last.Position, transformPosition, Mover.PositionOffset);

					if (_pathController.Path.UsePathfinding && pathfinding)
					{
						var pathFrom = GetPathPosition(first.Position, _pathController.transform.position);
						var pathTo = GetPathPosition(last.Position, transformPosition);
						var path = pathfinding.GetPath(mover.MovementLayer, pathFrom, pathTo, false);

						if (path.Count > 1)
						{
							for (var n = 1; n < path.Count; n++)
							{
								var node = path[n];
								var position = node + Mover.PositionOffset;

								HandleHelper.DrawLine(position, firstPosition, lineColor);

								firstPosition = position;
							}
						}
					}
					else
					{
						HandleHelper.DrawLine(lastPosition, firstPosition, lineColor);
					}
				}

				for (var i = 1; i < _pathController.Path.Nodes.Length; i++)
				{
					var previous = _pathController.Path.Nodes[i - 1];
					var current = _pathController.Path.Nodes[i];

					var previousPosition = GetPosition(previous.Position, i == 1 ? (Vector2)_pathController.transform.position : transformPosition, Mover.PositionOffset);
					var currentPosition = GetPosition(current.Position, transformPosition, Mover.PositionOffset);

					if (_pathController.Path.UsePathfinding && pathfinding)
					{
						var pathFrom = GetPathPosition(previous.Position, i == 1 ? (Vector2)_pathController.transform.position : transformPosition);
						var pathTo = GetPathPosition(current.Position, transformPosition);
						var path = pathfinding.GetPath(mover.MovementLayer, pathFrom, pathTo, false);

						if (path.Count > 1)
						{
							for (var n = 1; n < path.Count; n++)
							{
								var node = path[n];
								var position = node + Mover.PositionOffset;

								HandleHelper.DrawLine(position, previousPosition, lineColor);

								previousPosition = position;
							}
						}
					}
					else
					{
						HandleHelper.DrawLine(currentPosition, previousPosition, Color.cyan);
					}
				}

				for (var i = 0; i < _pathController.Path.Nodes.Length; i++)
				{
					var node = _pathController.Path.Nodes[i];
					var position = GetPosition(node.Position, i == 0 ? (Vector2)_pathController.transform.position : transformPosition, Mover.PositionOffset);
					var selectedPosition = HandleHelper.MoveHandle(position, Vector2.one, lineColor, fillColor, 0.25f);

					if (i != 0 && position != selectedPosition)
					{
						selectedPosition -= transformPosition;

						using (new UndoScope(_pathController, true))
							_pathController.Path.Nodes[i].Position = Vector2Int.FloorToInt(selectedPosition);
					}

					if (node.Delay > 0 && node.Direction != MovementDirection.None)
						HandleHelper.DrawArrow(selectedPosition, Direction.GetVector(node.Direction), 0.25f, Color.white);

					HandleHelper.DrawText(new Vector2(selectedPosition.x, selectedPosition.y + 0.05f), i.ToString(), TextAnchor.MiddleCenter, Color.white);
				}
			}
		}

		private Vector2 GetPosition(Vector2Int nodePosition, Vector2 transformPosition, Vector2 offset)
		{
			return GetPathPosition(nodePosition, transformPosition) + offset;
		}

		private Vector2Int GetPathPosition(Vector2Int nodePosition, Vector2 transformPosition)
		{
			return new Vector2Int(Mathf.FloorToInt(nodePosition.x + transformPosition.x), Mathf.FloorToInt(nodePosition.y + transformPosition.y));
		}
	}
}
