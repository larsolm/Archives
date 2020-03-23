using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(Ability))]
	class AbilityEditor : Editor
	{
		private SerializedProperty _traits;
		private SerializedProperty _worldInstructions;
		private SerializedProperty _battleInstructions;

		private void OnEnable()
		{
			_traits = serializedObject.FindProperty("Traits");
			_worldInstructions = serializedObject.FindProperty("WorldInstructions");
			_battleInstructions = serializedObject.FindProperty("BattleInstructions");
		}

		public override void OnInspectorGUI()
		{
			var back = GUILayout.Button(EditorHelper.BackContent, GUILayout.Width(60.0f));

			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_traits);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_worldInstructions);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_battleInstructions);
			}

			if (back)
				EditorHelper.Edit((target as Ability).Ecosystem);
		}
	}
}
