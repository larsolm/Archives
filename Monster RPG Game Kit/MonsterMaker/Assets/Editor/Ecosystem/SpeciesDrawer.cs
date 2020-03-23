using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomPropertyDrawer(typeof(Species))]
	public class SpeciesDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = EditorHelper.GetTooltip(fieldInfo);

			var nameRect = new Rect(position.x, position.y, position.width * 0.8f, EditorGUIUtility.singleLineHeight);
			var editRect = new Rect(nameRect.xMax + 5, position.y, position.width - nameRect.width - 5, EditorGUIUtility.singleLineHeight);

			var species = property.objectReferenceValue as Species;
			EditorGUI.LabelField(nameRect, label, new GUIContent(species.name, "The name of this species"));

			if (GUI.Button(editRect, EditorHelper.EditContent))
				EditorHelper.Edit(species);
		}
	}
}
