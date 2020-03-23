using System.Collections.Generic;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(BranchOnString))]
	public class BranchOnStringEditor : BranchInstructionEditor
	{
		private class NewBranchPopup : PopupWindowContent
		{
			private BranchOnStringEditor _editor;
			private string _newKey = "";
		
			private GUIContent _label = new GUIContent("New Branch", "Create new branch.");
		
			public NewBranchPopup(BranchOnStringEditor editor)
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
		
				var enter = GuiFields.TextEnterField("NewBranchKey", GUIContent.none, ref _newKey);
				var create = GUILayout.Button(EditorHelper.CreateContent);

				if ((enter || create) && !string.IsNullOrEmpty(_newKey))
				{
					_editor.AddBranch(_editor.target as BranchOnString, _newKey);
					editorWindow.Close();
				}
			}
		}
		
		private GUIContent _label = new GUIContent("", "The instruction that will be run on this branch.");
		private EditableList<string> _instructions = new EditableList<string>();
		private List<string> _branchKeys = new List<string>();

		protected override void OnEnable()
		{
			base.OnEnable();

			var instructions = _instructions.Setup(_branchKeys, "Instructions", "The instructions to run based on the variable.", false, false, false, true, true, DrawInstruction, RemoveInstruction);
			instructions.onAddDropdownCallback += AddBranchDropdown;

			UpdateBranchKeys(target as BranchOnString);
		}

		private void DrawInstruction(Rect rect, int index)
		{
			var branch = target as BranchOnString;
			var key = _branchKeys[index];
			var instruction = branch.Instructions[key];

			var keyRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
			var instructionRect = new Rect(keyRect.xMax + 5, rect.y, rect.width - keyRect.width - 5, keyRect.height);
			
			EditorGUI.LabelField(keyRect, key);

			branch.Instructions[key] = InstructionDrawer.Draw(instructionRect, instruction == null ? null : instruction.Set, instruction, _label);
		}
		
		private void RemoveInstruction(int index)
		{
			var branch = target as BranchOnString;
			var key = _branchKeys[index];
		
			branch.Instructions.Remove(key);
			UpdateBranchKeys(branch);
		}

		private void AddBranchDropdown(Rect rect, ReorderableList list)
		{
			rect.y += EditorGUIUtility.singleLineHeight;
			PopupWindow.Show(rect, new NewBranchPopup(this));
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			EditorGUILayout.Space();

			using (new UndoScope(target))
				_instructions.DrawList();
		}

		private void AddBranch(BranchOnString branch, string key)
		{
			branch.Instructions.Add(key, null);
			UpdateBranchKeys(branch);
		}
		
		private void UpdateBranchKeys(BranchOnString branch)
		{
			_branchKeys.Clear();
			foreach (var key in branch.Instructions.Keys)
				_branchKeys.Add(key);
			
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(branch));
		}
	}
}
