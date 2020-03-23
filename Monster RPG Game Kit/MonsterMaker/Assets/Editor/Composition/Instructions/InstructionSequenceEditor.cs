using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(InstructionSequence))]
	public class InstructionSequenceEditor : Editor
	{
		private EditableList<Instruction> _instructions = new EditableList<Instruction>();
		private TypeSelectInfo _instructionTypes;

		private GUIContent _label = new GUIContent("", "The type of instruction to run.");

		private void OnEnable()
		{
			var instructions = _instructions.Setup(serializedObject.FindProperty("Instructions"), null, null, false, true, false, true, true, DrawElement);
			instructions.onAddCallback += AddInstruction;
		}
		
		private void DrawElement(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			var instruction = element.objectReferenceValue as Instruction;
			var set = (target as InstructionSequence).Set;
			element.objectReferenceValue = InstructionDrawer.Draw(rect, set, instruction, _label);
		}
		
		private void AddInstruction(ReorderableList data)
		{
			var sequence = target as InstructionSequence;
			sequence.Instructions.Add(null);
		}

		public override void OnInspectorGUI()
		{
			InstructionBreadcrumbsDrawer.Draw();

			using (new UndoScope(serializedObject))
				_instructions.DrawList();
		}
	}
}
