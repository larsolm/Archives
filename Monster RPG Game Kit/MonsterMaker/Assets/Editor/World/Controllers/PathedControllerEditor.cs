using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(PathedController))]
	class PathedControllerEditor : Editor
	{
		private SerializedProperty _path;
		private SerializedProperty _loop;
		private SerializedProperty _complete;
		private SerializedProperty _usePathfinding;
		private SerializedProperty _findAlternate;
		private GUIStyle _textStyle = new GUIStyle();

		void OnEnable()
		{
			_path = serializedObject.FindProperty("Path");
			_loop = serializedObject.FindProperty("OnLoopCompleted");
			_complete = serializedObject.FindProperty("OnPathCompleted");
			_usePathfinding = serializedObject.FindProperty("UsePathfinding");
			_findAlternate = serializedObject.FindProperty("FindAlternateRoutes");

			_textStyle.normal.textColor = Color.black;
			_textStyle.fontStyle = FontStyle.Bold;
			_textStyle.alignment = TextAnchor.MiddleCenter;
		}

		public override void OnInspectorGUI()
		{
			var createPathfinding = false;

			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_loop);
				EditorGUILayout.PropertyField(_complete);
				EditorGUILayout.PropertyField(_usePathfinding);

				if (_usePathfinding.boolValue)
				{
					EditorGUILayout.PropertyField(_findAlternate);

					using (new EditorGUILayout.HorizontalScope())
					{
						var pathfinding = ComponentFinder.GetComponentInScene<Pathfinding>((target as PathedController).gameObject.scene.buildIndex);
						if (pathfinding == null)
						{
							EditorGUILayout.HelpBox("In order to use pathfinding this scene's map object must have a pathfinding component attached.", MessageType.Warning, true);

							createPathfinding = GUILayout.Button("Create\nNow", GUILayout.Height(39));
						}
					}
				}

				EditorGUILayout.PropertyField(_path);
			}

			if (createPathfinding)
			{

				var properties = ComponentFinder.GetComponentInScene<MapProperties>((target as PathedController).gameObject.scene.buildIndex);
				if (properties)
				{
					var path = Undo.AddComponent<Pathfinding>(properties.gameObject);
					path.RegenerateNodes();
				}
			}
		}

		private void OnSceneGUI()
		{
			var controller = target as PathedController;
			if (controller.Path.Nodes.Length > 0)
			{
				var pathfinding = ComponentFinder.GetComponentInScene<Pathfinding>(controller.gameObject.scene.buildIndex);
				var mover = controller.GetComponent<Mover>();

				Handles.color = Color.cyan;

				if (controller.Path.Nodes.Length > 2 && controller.Path.Type == Path.PathType.Loop)
				{
					var first = controller.Path.Nodes[0];
					var last = controller.Path.Nodes[controller.Path.Nodes.Length - 1];

					var firstPosition = GetPosition(first.Position, controller.transform.position, mover.PositionOffset);
					var lastPosition = GetPosition(last.Position, controller.transform.position, mover.PositionOffset);

					if (controller.UsePathfinding && pathfinding)
					{
						var pathFrom = GetPathPosition(first.Position, controller.transform.position);
						var pathTo = GetPathPosition(last.Position, controller.transform.position);
						var path = pathfinding.GetPath(mover, pathFrom, pathTo, controller.FindAlternateRoutes);

						if (path.Count > 1)
						{
							for (var n = 1; n < path.Count; n++)
							{
								var node = path[n];
								var position = node + mover.PositionOffset;

								Handles.DrawLine(position, firstPosition);

								firstPosition = position;
							}
						}
					}
					else
					{
						Handles.DrawLine(lastPosition, firstPosition);
					}
				}

				for (var i = 1; i < controller.Path.Nodes.Length; i++)
				{
					var previous = controller.Path.Nodes[i - 1];
					var current = controller.Path.Nodes[i];

					var previousPosition = GetPosition(previous.Position, controller.transform.position, mover.PositionOffset);
					var currentPosition = GetPosition(current.Position, controller.transform.position, mover.PositionOffset);

					if (controller.UsePathfinding && pathfinding)
					{
						var pathFrom = GetPathPosition(previous.Position, controller.transform.position);
						var pathTo = GetPathPosition(current.Position, controller.transform.position);
						var path = pathfinding.GetPath(mover, pathFrom, pathTo, controller.FindAlternateRoutes);

						if (path.Count > 1)
						{
							for (var n = 1; n < path.Count; n++)
							{
								var node = path[n];
								var position = node + mover.PositionOffset;

								Handles.DrawLine(position, previousPosition);

								previousPosition = position;
							}
						}
					}
					else
					{
						Handles.DrawLine(currentPosition, previousPosition);
					}

					var selectedPosition = Handles.FreeMoveHandle(currentPosition, Quaternion.identity, 0.25f, Vector3.one, DrawRect);

					if (currentPosition != selectedPosition)
					{
						using (new UndoScope(serializedObject))
						{
							selectedPosition -= controller.transform.position;

							var node = _path.FindPropertyRelative(string.Format("Nodes.Array.data[{0}].Position", i));
							node.vector2IntValue = Vector2Int.FloorToInt(new Vector2(selectedPosition.x, selectedPosition.y));
						}
					}
				}

				for (var i = 1; i < controller.Path.Nodes.Length; i++)
				{
					var position = GetPosition(controller.Path.Nodes[i].Position, controller.transform.position, mover.PositionOffset);
					Handles.Label(new Vector2(position.x, position.y + 0.075f), i.ToString(), _textStyle);
				}
			}
		}

		private void DrawRect(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			Handles.color = new Color(0.0f, 1.0f, 1.0f, 0.2f);
			Handles.DrawSolidDisc(position, Vector3.back, size);
			Handles.color = Color.cyan;
			Handles.DrawWireDisc(position, Vector3.back, size);
			Handles.CircleHandleCap(controlID, position, rotation, size, eventType);
		}

		private Vector3 GetPosition(Vector2Int nodePosition, Vector3 transformPosition, Vector2 offset)
		{
			return GetPathPosition(nodePosition, transformPosition) + offset;
		}

		private Vector2Int GetPathPosition(Vector2Int nodePosition, Vector3 transformPosition)
		{
			return new Vector2Int(Mathf.FloorToInt(nodePosition.x + transformPosition.x), Mathf.FloorToInt(nodePosition.y + transformPosition.y));
		}
	}
}
