using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	public class PathControl : PropertyControl
	{
		private static readonly IconButton _addButton = new IconButton(IconButton.Add, "Add a node to the Path");
		private static readonly IconButton _removeButton = new IconButton(IconButton.Remove, "Remove this Path node");
		private static readonly Label _nodesContent = new Label(typeof(Path), nameof(Path.Nodes));

		private SerializedProperty _type;
		private SerializedProperty _repeatCount;
		private SerializedProperty _absolute;
		private SerializedProperty _usePathfinding;
		private SerializedProperty _findAlternate;
		private PropertyListControl _nodesControl = new PropertyListControl();

		public override void Setup(SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_type = property.FindPropertyRelative(nameof(Path.Type));
			_absolute = property.FindPropertyRelative(nameof(Path.UseAbsolutePositioning));
			_usePathfinding = property.FindPropertyRelative(nameof(Path.UsePathfinding));
			_findAlternate = property.FindPropertyRelative(nameof(Path.FindAlternateRoutes));
			_repeatCount = property.FindPropertyRelative(nameof(Path.RepeatCount));

			_nodesControl.Setup(property.FindPropertyRelative(nameof(Path.Nodes)))
				.MakeDrawable(DrawNode)
				.MakeAddable(_addButton)
				.MakeRemovable(_removeButton, RemoveNode)
				.MakeCollapsable(property.serializedObject.targetObject.GetType() + "." + property.propertyPath + ".IsOpen")
				.MakeCustomHeight(GetNodeHeight);
		}

		public override float GetHeight(SerializedProperty property, GUIContent label)
		{
			return (_usePathfinding.boolValue ? 6 : 5) * RectHelper.LineHeight + _nodesControl.GetHeight();
		}

		public override void Draw(Rect position, SerializedProperty property, GUIContent label)
		{
			var labelRect = RectHelper.TakeLine(ref position);
			var typeRect = RectHelper.TakeLine(ref position);
			var repeatRect = RectHelper.TakeLine(ref position);
			var absoluteRect = RectHelper.TakeLine(ref position);
			var pathfindingRect = RectHelper.TakeLine(ref position);

			EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);
			EditorGUI.PropertyField(typeRect, _type);
			EditorGUI.PropertyField(repeatRect, _repeatCount);
			EditorGUI.PropertyField(absoluteRect, _absolute);
			EditorGUI.PropertyField(pathfindingRect, _usePathfinding);

			if (_usePathfinding.boolValue)
				EditorGUI.PropertyField(RectHelper.TakeLine(ref position), _findAlternate);

			_nodesControl.Draw(position, _nodesContent.Content);
		}

		private float GetNodeHeight(int index)
		{
			return 3 * RectHelper.LineHeight;
		}

		private void DrawNode(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			var positionRect = RectHelper.TakeLine(ref rect);
			var delayRect = RectHelper.TakeLine(ref rect);
			var directionRect = RectHelper.TakeLine(ref rect);

			GUI.enabled = index != 0;
			EditorGUI.PropertyField(positionRect, element.FindPropertyRelative(nameof(Path.NodeData.Position)));
			GUI.enabled = true;

			EditorGUI.PropertyField(delayRect, element.FindPropertyRelative(nameof(Path.NodeData.Delay)));
			EditorGUI.PropertyField(directionRect, element.FindPropertyRelative(nameof(Path.NodeData.Direction)));
		}

		private void RemoveNode(SerializedProperty property, int index)
		{
			if (index > 0)
				_nodesControl.DoDefaultRemove(index);
		}
	}

	[CustomPropertyDrawer(typeof(Path))]
	public class PathDrawer : ControlDrawer<PathControl>
	{
	}
}
