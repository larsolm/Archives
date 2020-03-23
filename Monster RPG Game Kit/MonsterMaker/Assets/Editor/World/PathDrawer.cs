using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomPropertyDrawer(typeof(Path))]
	public class PathDrawer : PropertyDrawer
	{
		private struct Data
		{
			public SerializedProperty Type;
			public SerializedProperty RepeatCount;
			public EditableList<Path.Node> List;

			public int Remove;
		}

		private Dictionary<string, Data> _datas = new Dictionary<string, Data>();
		private Data _currentData;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			Init(property);

			return 5 * (EditorGUIUtility.singleLineHeight + 5) + (_currentData.List.Count * GetHeight(0));
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Init(property);

			var typeRect = new Rect(position.x + 10, position.y, position.width - 10, EditorGUIUtility.singleLineHeight);
			var repeatRect = new Rect(position.x + 10, typeRect.yMax + 5, position.width - 10, EditorGUIUtility.singleLineHeight);
			var listRect = new Rect(position.x + 10, repeatRect.yMax + 5, position.width - 10, _currentData.List.Height);

			EditorGUI.PropertyField(typeRect, _currentData.Type);
			EditorGUI.PropertyField(repeatRect, _currentData.RepeatCount);

			_currentData.List.DrawList(listRect);

			if (_currentData.Remove > 0)
				property.DeleteArrayElementAtIndex(_currentData.Remove);
		}

		private void Init(SerializedProperty property)
		{
			var path = property.serializedObject.targetObject.name + property.propertyPath;

			if (!_datas.TryGetValue(path, out _currentData))
			{
				_currentData = new Data
				{
					Type = property.FindPropertyRelative("Type"),
					RepeatCount = property.FindPropertyRelative("RepeatCount"),
					List = new EditableList<Path.Node>()
				};

				var nodes = _currentData.List.Setup(property.FindPropertyRelative("Nodes"), null, null, true, true, false, true, true, DrawNode, RemoveNode);
				nodes.elementHeightCallback += GetHeight;

				_datas.Add(path, _currentData);
			}

			_currentData.Remove = 0;
		}

		private float GetHeight(int index)
		{
			return _currentData.List.Visible ? 2 * (EditorGUIUtility.singleLineHeight + 5): 0;
		}

		private void DrawNode(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			var delayRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
			var positionRect = new Rect(rect.x, delayRect.yMax + EditorGUIUtility.standardVerticalSpacing, rect.width, EditorGUIUtility.singleLineHeight);

			EditorGUI.PropertyField(delayRect, element.FindPropertyRelative("MoveDelay"));
			GUI.enabled = index != 0;
			EditorGUI.PropertyField(positionRect, element.FindPropertyRelative("Position"));
			GUI.enabled = true;
		}

		private void RemoveNode(SerializedProperty property, int index)
		{
			_currentData.Remove = index;
		}
	}
}
