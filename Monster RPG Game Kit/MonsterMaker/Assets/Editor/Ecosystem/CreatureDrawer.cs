using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomPropertyDrawer(typeof(Creature))]
	public class CreatureDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var nameRect = new Rect(position.x, position.y, position.width * 0.8f, EditorGUIUtility.singleLineHeight);
			var editRect = new Rect(nameRect.xMax + 5, position.y, position.width - nameRect.width - 5, EditorGUIUtility.singleLineHeight);

			var creature = property.objectReferenceValue as Creature;
			EditorGUI.LabelField(nameRect, creature.Species.name);

			if (GUI.Button(editRect, EditorHelper.EditContent))
				EditorHelper.Edit(creature);
		}
	}
}
