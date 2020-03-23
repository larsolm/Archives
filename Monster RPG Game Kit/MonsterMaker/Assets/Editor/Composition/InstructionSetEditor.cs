using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(InstructionSet))]
	public class InstructionSetEditor : Editor
	{
		private EditableList<Instruction> _instructions = new EditableList<Instruction>();
		private GenericMenu _addMenu = new GenericMenu();
		private TypeSelectInfo _instructionTypes;

		private Instruction _toEdit = null;
		private int _toRemove = -1;

		private void OnEnable()
		{
			InstructionBreadcrumbs.Reset();

			var instructions = _instructions.Setup(serializedObject.FindProperty("Instructions"), null, null, false, true, false, true, true, DrawInstruction, RemoveInstruction);
			instructions.onAddDropdownCallback += AddInstructionDropdown;

			_instructionTypes = TypeFinder.GetDerivedTypes<Instruction>("");
			
			foreach (var type in _instructionTypes.Types)
				_addMenu.AddItem(new GUIContent(type.Name, "The type of instruction to create."), false, AddInstruction, type);
		}

		private void DrawInstruction(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			var instruction = element.objectReferenceValue as Instruction;

			if (instruction != null)
			{
				var nameRect = new Rect(rect.x, rect.y, rect.width * 0.8f, EditorGUIUtility.singleLineHeight);
				var editRect = new Rect(nameRect.xMax + 5, rect.y, rect.width * 0.2f - 5, nameRect.height);

				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					instruction.name = EditorGUI.DelayedTextField(nameRect, instruction.name);

					if (changes.changed)
						InstructionDrawer.InstructionSetChanged(instruction.Set);
				}

				if (GUI.Button(editRect, EditorHelper.EditContent))
					_toEdit = instruction;
			}
		}

		private void AddInstructionDropdown(Rect rect, ReorderableList list)
		{
			_addMenu.ShowAsContext();
		}

		private void RemoveInstruction(SerializedProperty property, int index)
		{
			_toRemove = index;
		}

		public override void OnInspectorGUI()
		{
			InstructionBreadcrumbsDrawer.Draw();

			var set = target as InstructionSet;
			var design = GUILayout.Button("Show Designer");

			_toEdit = null;
			_toRemove = -1;

			EditorGUILayout.Space();
			EditorHelper.Separator(Color.grey);
			EditorGUILayout.Space();

			using (new UndoScope(serializedObject))
				_instructions.DrawList();

			if (_toEdit != null)
				InstructionBreadcrumbs.Edit(_toEdit);

			if (_toRemove >= 0)
				Remove(set, _toRemove);

			if (design)
				Design(set);
		}

		private void AddInstruction(object data)
		{
			var type = data as Type;
			var set = target as InstructionSet;

			using (new UndoScope(set))
			{
				var instruction = InstructionDrawer.Create(set, null, type);
				Undo.RegisterCreatedObjectUndo(instruction, "Undo create instruction");
			}
		}

		private void Remove(InstructionSet set, int index)
		{
			using (new UndoScope(set))
			{
				var instruction = set.Instructions[index];
				if (instruction != null)
					Undo.DestroyObjectImmediate(instruction);

				set.Instructions.RemoveAt(index);

				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(set));
				InstructionDrawer.InstructionSetChanged(set);
			}
		}

		private void Design(InstructionSet set)
		{
			CreateInstance<InstructionSetWindow>().Show(set);
		}
	}
}
