using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(BranchOnInt))]
	public class BranchOnIntEditor : BranchInstructionEditor
	{
		private EditableList<Instruction> _instructions = new EditableList<Instruction>();
		private GUIContent _label = new GUIContent("", "The instruction that will be run on this branch.");

		protected override void OnEnable()
		{
			base.OnEnable();

			_instructions.Setup(serializedObject.FindProperty("Instructions"), null, null, false, true, false, true, true, DrawInstruction);
		}
		
		private void DrawInstruction(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			var instruction = element.objectReferenceValue as Instruction;
			var indexRect = new Rect(rect.x, rect.y, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);
			var instructionRect = new Rect(indexRect.xMax + 5, rect.y, rect.width - indexRect.width - 5, indexRect.height);

			EditorGUI.LabelField(indexRect, index.ToString());
			element.objectReferenceValue = InstructionDrawer.Draw(instructionRect, instruction == null ? null : instruction.Set, instruction, _label);
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();

			using (new UndoScope(serializedObject))
				_instructions.DrawList();
		}
	}
}
