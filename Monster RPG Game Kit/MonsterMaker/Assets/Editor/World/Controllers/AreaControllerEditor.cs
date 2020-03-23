using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(AreaController))]
	class AreaControllerEditor : Editor
	{
		private SerializedProperty _left;
		private SerializedProperty _right;
		private SerializedProperty _up;
		private SerializedProperty _down;
		private SerializedProperty _delay;

		void OnEnable()
		{
			_left = serializedObject.FindProperty("LeftDistance");
			_right = serializedObject.FindProperty("RightDistance");
			_up = serializedObject.FindProperty("UpDistance");
			_down = serializedObject.FindProperty("DownDistance");
			_delay = serializedObject.FindProperty("MovementDelay");
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_left);
				EditorGUILayout.PropertyField(_right);
				EditorGUILayout.PropertyField(_up);
				EditorGUILayout.PropertyField(_down);
				EditorGUILayout.PropertyField(_delay);

				Validate();
			}
		}

		private void OnSceneGUI()
		{
			var controller = target as AreaController;
			var mover = controller.GetComponent<Mover>();
			var area = new Rect(controller.transform.position.x - mover.PositionOffset.x - controller.LeftDistance, controller.transform.position.y - mover.PositionOffset.y - controller.DownDistance, 1 + controller.RightDistance + controller.LeftDistance, 1 + controller.UpDistance + controller.DownDistance);

			Handles.DrawSolidRectangleWithOutline(area, new Color(0.0f, 1.0f, 1.0f, 0.2f), Color.cyan);

			var leftPosition = new Vector3(area.xMin, area.center.y);
			var rightPosition = new Vector3(area.xMax, area.center.y);
			var topPosition = new Vector3(area.center.x, area.yMax);
			var bottomPosition = new Vector3(area.center.x, area.yMin);
			var topLeftPosition = new Vector3(area.xMin, area.yMax);
			var topRightPosition = new Vector3(area.xMax, area.yMax);
			var bottomLeftPosition = new Vector3(area.xMin, area.yMin);
			var bottomRightPosition = new Vector3(area.xMax, area.yMin);

			var selectedLeftPosition = Handles.FreeMoveHandle(leftPosition, Quaternion.identity, 0.05f, Vector3.one, DrawRect);
			var selectedRightPosition = Handles.FreeMoveHandle(rightPosition, Quaternion.identity, 0.05f, Vector3.one, DrawRect);
			var selectedTopPosition = Handles.FreeMoveHandle(topPosition, Quaternion.identity, 0.05f, Vector3.one, DrawRect);
			var selectedBottomPosition = Handles.FreeMoveHandle(bottomPosition, Quaternion.identity, 0.05f, Vector3.one, DrawRect);
			var selectedTopLeftPosition = Handles.FreeMoveHandle(topLeftPosition, Quaternion.identity, 0.05f, Vector3.one, DrawRect);
			var selectedTopRightPosition = Handles.FreeMoveHandle(topRightPosition, Quaternion.identity, 0.05f, Vector3.one, DrawRect);
			var selectedBottomLeftPosition = Handles.FreeMoveHandle(bottomLeftPosition, Quaternion.identity, 0.05f, Vector3.one, DrawRect);
			var selectedBottomRightPosition = Handles.FreeMoveHandle(bottomRightPosition, Quaternion.identity, 0.05f, Vector3.one, DrawRect);

			using (new UndoScope(serializedObject))
			{
				var left = GetHorizontalMovement(selectedLeftPosition, leftPosition);
				var right = GetHorizontalMovement(selectedRightPosition, rightPosition);
				var top = GetVerticalMovement(selectedTopPosition, topPosition);
				var bottom = GetVerticalMovement(selectedBottomPosition, bottomPosition);
				var topLeft = GetDirectionalMovement(selectedTopLeftPosition, topLeftPosition);
				var topRight = GetDirectionalMovement(selectedTopRightPosition, topRightPosition);
				var bottomLeft = GetDirectionalMovement(selectedBottomLeftPosition, bottomLeftPosition);
				var bottomRight = GetDirectionalMovement(selectedBottomRightPosition, bottomRightPosition);

				_left.intValue -= left + topLeft.x + bottomLeft.x;
				_right.intValue += right + topRight.x + bottomRight.x;
				_up.intValue += top + topLeft.y + topRight.y;
				_down.intValue -= bottom + bottomLeft.y + bottomRight.y;

				Validate();
			}
		}

		private void DrawRect(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			Handles.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
			Handles.DrawSolidDisc(position, Vector3.back, size);
			Handles.color = Color.white;
			Handles.DrawWireDisc(position, Vector3.back, size);
			Handles.CircleHandleCap(controlID, position, rotation, size, eventType);
		}

		private void Validate()
		{
			_left.intValue = Mathf.Max(_left.intValue, 0);
			_right.intValue = Mathf.Max(_right.intValue, 0);
			_up.intValue = Mathf.Max(_up.intValue, 0);
			_down.intValue = Mathf.Max(_down.intValue, 0);
			_delay.floatValue = Mathf.Max(_delay.floatValue, 0.0f);
		}

		private int GetHorizontalMovement(Vector3 selected, Vector3 current)
		{
			var movement = selected - current;
			return Mathf.RoundToInt(movement.x);
		}

		private int GetVerticalMovement(Vector3 selected, Vector3 current)
		{
			var movement = selected - current;
			return Mathf.RoundToInt(movement.y);
		}

		private Vector2Int GetDirectionalMovement(Vector3 selected, Vector3 current)
		{
			var movement = selected - current;
			return Vector2Int.RoundToInt(movement);
		}
	}
}
