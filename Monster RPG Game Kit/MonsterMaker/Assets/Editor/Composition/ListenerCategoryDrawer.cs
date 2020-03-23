using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomPropertyDrawer(typeof(ListenerCategoryAttribute))]
	public class ListenerCategoryDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = EditorHelper.GetTooltip(fieldInfo);

			var popupRect = new Rect(position.x, position.y, position.width * 0.8f, EditorGUIUtility.singleLineHeight);
			var editRect = new Rect(popupRect.xMax + 5, position.y, position.width - popupRect.width - 5, EditorGUIUtility.singleLineHeight);

			var index = UiPreferences.Instance.ListenerCategories.IndexOf(property.stringValue);
			var selectedIndex = EditorGUI.Popup(popupRect, label.text, index, UiPreferences.Instance.ListenerCategories.ToArray());

			if (selectedIndex != index)
			{
				property.stringValue = UiPreferences.Instance.ListenerCategories[selectedIndex];
				property.serializedObject.ApplyModifiedProperties();
			}

			if (GUI.Button(editRect, EditorHelper.EditContent))
				ListenerCategoryWindow.ShowWindow();
		}
	}
}
