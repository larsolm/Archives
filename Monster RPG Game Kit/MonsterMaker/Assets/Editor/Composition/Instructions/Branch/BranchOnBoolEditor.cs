using UnityEditor;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(BranchOnBool))]
	public class BranchOnBoolEditor : BranchInstructionEditor
	{
		private SerializedProperty _true;
		private SerializedProperty _false;

		protected override void OnEnable()
		{
			base.OnEnable();

			_true = serializedObject.FindProperty("OnTrue");
			_false = serializedObject.FindProperty("OnFalse");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_true);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_false);
			}
		}
	}
}
