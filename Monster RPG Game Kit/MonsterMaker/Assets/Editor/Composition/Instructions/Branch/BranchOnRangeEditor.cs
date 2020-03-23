using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class BranchOnRangeEditor<T> : BranchInstructionEditor where T : IComparable<T>
	{
		private class NewRangePopup : PopupWindowContent
		{
			private BranchOnRangeEditor<T> _editor;
			private GUIContent _label = new GUIContent("New Range", "Create new range.");
			private T _newRange;

			public NewRangePopup(BranchOnRangeEditor<T> editor)
			{
				_editor = editor;
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, EditorGUIUtility.singleLineHeight * 4);
			}

			public override void OnGUI(Rect rect)
			{
				EditorGUILayout.LabelField(_label);

				var enter = _editor.DrawNewRangeField(ref _newRange);
				var create = GUILayout.Button(EditorHelper.CreateContent);

				if (enter || create)
				{
					_editor.AddRange(_newRange);
					editorWindow.Close();
				}
			}
		}

		private EditableList<BranchOnRange<T>.Range> _instructions = new EditableList<BranchOnRange<T>.Range>();
		
		private GUIContent _label = new GUIContent("", "The instruction that will be run on this range.");

		protected abstract bool DrawNewRangeField(ref T range);
		protected abstract T DrawRange(Rect rect, T range, BranchOnRange<T>.Range min, BranchOnRange<T>.Range max);

		protected override void OnEnable()
		{
			base.OnEnable();

			var instructions = _instructions.Setup((target as BranchOnRange<T>).Instructions, "Instructions", "The list of instructions to run based on the range state.", false, false, false, true, true, DrawRange, RemoveRange);
			instructions.onAddDropdownCallback += AddRangeDropdown;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			using (new UndoScope(target))
				_instructions.DrawList();
		}
		
		private void DrawRange(Rect rect, int index)
		{
			var branch = target as BranchOnRange<T>;
			var range = branch.Instructions[index];
			var min = index > 0 ? branch.Instructions[index - 1] : null;
			var max = index < branch.Instructions.Count - 1 ? branch.Instructions[index + 1] : null;
			var valueRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
			var instructionRect = new Rect(valueRect.xMax + 5, rect.y, rect.width - valueRect.width - 5, valueRect.height);

			var value = DrawRange(valueRect, range.Value, min, max);
			if (!branch.Instructions.Any(other => other.Value.CompareTo(value) == 0))
			{
				range.Value = value;
				branch.Instructions.Sort((first,second) => first.Value.CompareTo(second.Value));
			}

			range.Instruction = InstructionDrawer.Draw(instructionRect, branch.Set, range.Instruction, _label);
		}

		private void AddRangeDropdown(Rect rect, ReorderableList list)
		{
			rect.y += EditorGUIUtility.singleLineHeight;
			PopupWindow.Show(rect, new NewRangePopup(this));
		}

		private void RemoveRange(int index)
		{
			var branch = target as BranchOnRange<T>;
			branch.Instructions.RemoveAt(index);
		}

		public void AddRange(T range)
		{
			var branch = target as BranchOnRange<T>;
			var newRange = new BranchOnRange<T>.Range { Value = range, Instruction = null };

			if (!branch.Instructions.Any(other => other.Value.CompareTo(newRange.Value) == 0))
			{
				using (new UndoScope(branch))
				{
					branch.Instructions.Add(newRange);
					branch.Instructions.Sort((first, second) => first.Value.CompareTo(second.Value));
				}
			}
		}
	}
}
