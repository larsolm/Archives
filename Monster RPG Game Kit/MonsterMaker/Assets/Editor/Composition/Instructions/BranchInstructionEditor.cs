using UnityEditor;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class BranchInstructionEditor : Editor
	{
		private SerializedProperty _variable;
		private SerializedProperty _default;
		private SerializedProperty _unset;

		protected virtual void OnEnable()
		{
			_variable = serializedObject.FindProperty("Variable");
			_default = serializedObject.FindProperty("Default");
			_unset = serializedObject.FindProperty("Unset");
		}

		public override void OnInspectorGUI()
		{
			InstructionBreadcrumbsDrawer.Draw();

			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.PropertyField(_variable);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_default);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_unset);
			}
		}
	}
}
