using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class SelectionInstructionEditor<T> : Editor where T : SelectionOption
	{
		private SerializedProperty _category;
		private SerializedProperty _message;
		private SerializedProperty _variable;
		private SerializedProperty _cancel;

		private EditableList<T> _instructions = new EditableList<T>();

		private GUIContent _selection = new GUIContent("", "The object to display for this option.");
		private GUIContent _instruction = new GUIContent("", "The instruction to run for this option.");

		private void OnEnable()
		{
			_category = serializedObject.FindProperty("Category");
			_message = serializedObject.FindProperty("Message");
			_variable = serializedObject.FindProperty("Variable");
			_cancel = serializedObject.FindProperty("CancelInstruction");
			_instructions.Setup(serializedObject.FindProperty("StringOptions"), null, null, false, true, false, true, true, DrawOption);
		}

		private void DrawOption(Rect rect, SerializedProperty property, int index)
		{
			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				var element = property.GetArrayElementAtIndex(index);
				var selection = element.FindPropertyRelative("Selection");
				var instruction = element.FindPropertyRelative("Instruction");

				var selectionRect = new Rect(rect.x, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight);
				var instructionRect = new Rect(selectionRect.xMax + 5, rect.y, rect.width - selectionRect.width - 5, rect.height);

				EditorGUI.PropertyField(selectionRect, selection, _selection);
				EditorGUI.PropertyField(instructionRect, instruction, _instruction);
			}
		}

		public override void OnInspectorGUI()
		{
			InstructionBreadcrumbsDrawer.Draw();

			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.PropertyField(_category);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_message);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_variable);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_cancel);
				EditorGUILayout.Space();

				_instructions.DrawList();
			}
		}
	}
}
