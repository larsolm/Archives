using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	[CustomEditor(typeof(AreaController))]
	class AreaControllerEditor : Editor
	{
		private AreaController _areaController;

		private SerializedProperty _left;
		private SerializedProperty _right;
		private SerializedProperty _up;
		private SerializedProperty _down;
		private SerializedProperty _delay;

		void OnEnable()
		{
			_areaController = target as AreaController;

			_left = serializedObject.FindProperty(nameof(AreaController.LeftDistance));
			_right = serializedObject.FindProperty(nameof(AreaController.RightDistance));
			_up = serializedObject.FindProperty(nameof(AreaController.UpDistance));
			_down = serializedObject.FindProperty(nameof(AreaController.DownDistance));
			_delay = serializedObject.FindProperty(nameof(AreaController.MovementDelay));
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
			var area = new Rect(_areaController.transform.position.x - Mover.PositionOffset.x - _areaController.LeftDistance, _areaController.transform.position.y - Mover.PositionOffset.y - _areaController.DownDistance, 1 + _areaController.RightDistance + _areaController.LeftDistance, 1 + _areaController.UpDistance + _areaController.DownDistance);

			Handles.DrawSolidRectangleWithOutline(area, new Color(0.0f, 1.0f, 1.0f, 0.1f), Color.cyan);

			var bounds = HandleHelper.ScaleHandles(area, Vector2.one, Color.white, Color.white);

			using (new UndoScope(serializedObject))
			{
				_left.intValue -= Mathf.RoundToInt(bounds.xMin - area.xMin);
				_right.intValue += Mathf.RoundToInt(bounds.xMax - area.xMax);
				_up.intValue += Mathf.RoundToInt(bounds.yMax - area.yMax);
				_down.intValue -= Mathf.RoundToInt(bounds.yMin - area.yMin);

				Validate();
			}
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
