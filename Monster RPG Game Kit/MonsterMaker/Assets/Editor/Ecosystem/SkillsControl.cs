using System;
using System.Collections.Generic;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class SkillsControl
	{
		private Species _species;
		private EditableList<Skill> _skills;
		private List<InstructionInputList> _inputLists = new List<InstructionInputList>();
		private static string[] _limits = new string[] { "Once", "Custom Limit", "Unlimited" };

		public SkillsControl(Species species)
		{
			_species = species;
			_skills = new EditableList<Skill>();
			var list = _skills.Setup(species.Skills, "Skills", "", false, true, false, true, true, DrawSkill);
			list.onAddDropdownCallback += AddSkill;
			list.elementHeightCallback += GetSkillHeight;

			for (var i = 0; i < species.Skills.Count; i++)
			{
				var skill = species.Skills[i];
				SkillAdded(skill);
				RefreshInputs(_inputLists[i], skill.Instruction);
			}
		}

		private class AddPopup : PopupWindowContent
		{
			private SkillsControl _control;

			public AddPopup(SkillsControl control)
			{
				_control = control;
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, EditorGUIUtility.singleLineHeight * 4);
			}

			public override void OnGUI(Rect rect)
			{
				EditorGUILayout.LabelField(_label);

				var enter = GuiFields.TextEnterField("NewSkillName", GUIContent.none, ref _newName);
				var create = GUILayout.Button(EditorHelper.CreateContent);

				if ((create || enter) && !string.IsNullOrEmpty(_newName))
				{
					CreateSkill(_newName);
					editorWindow.Close();
				}
			}

			private GUIContent _label = new GUIContent("New Skill", "Add a new learnable skill to the species");
			private string _newName = "Name";

			private void CreateSkill(string name)
			{
				var skill = new Skill();
				skill.Name = name;
				_control._species.Skills.Add(skill);
			}
		}

		private void AddSkill(Rect rect, ReorderableList list)
		{
			rect.y += EditorGUIUtility.singleLineHeight;
			PopupWindow.Show(rect, new AddPopup(this));
		}

		private void SkillAdded(Skill skill)
		{
			_inputLists.Add(new InstructionInputList());
		}

		private float GetSkillHeight(int index)
		{
			return (EditorGUIUtility.singleLineHeight + 5) * (4 + _species.Skills[index].Instruction.Inputs.Count);
		}

		private void DrawSkill(Rect rect, int index)
		{
			var skill = _species.Skills[index];
			var inputs = _inputLists[index];

			var labelRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
			var conditionRect = new Rect(rect.x + 10, labelRect.yMax + 5, rect.width - 10, EditorGUIUtility.singleLineHeight);
			var limitRect = new Rect(rect.x + 10, conditionRect.yMax + 5, rect.width * 0.75f, EditorGUIUtility.singleLineHeight);
			var limitAmountRect = new Rect(limitRect.xMax + 5, limitRect.y, rect.width - limitRect.width - 15, limitRect.height);
			var instructionRect = new Rect(rect.x + 10, limitRect.yMax + 5, rect.width - 10, EditorGUIUtility.singleLineHeight);
			var inputsRect = new Rect(rect.x + 10, instructionRect.yMax + 5, rect.width - 10, EditorGUIUtility.singleLineHeight);

			EditorGUI.LabelField(labelRect, skill.Name, EditorStyles.boldLabel);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				conditionRect = EditorGUI.PrefixLabel(conditionRect, new GUIContent("Condition"));
				var condition = EditorGUI.DelayedTextField(conditionRect, skill.Condition.Statement);

				if (changes.changed)
				{
					skill.Condition.Statement = condition;
					skill.UpdateTriggers();
				}
			}

			var limitIndex = EditorGUI.Popup(limitRect, "Learn Limit", skill.LearnLimit == 0 ? 2 : (skill.LearnLimit == 1 ? 0 : 1), _limits);

			if (limitIndex == 0)
			{
				skill.LearnLimit = 1;
			}
			else if (limitIndex == 1)
			{
				if (skill.LearnLimit == 0 || skill.LearnLimit == 1)
					skill.LearnLimit = 10;

				skill.LearnLimit = EditorGUI.IntField(limitAmountRect, skill.LearnLimit);
			}
			else if (limitIndex == 2)
			{
				skill.LearnLimit = 0;
			}

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				instructionRect = EditorGUI.PrefixLabel(instructionRect, new GUIContent("Instruction"));
				skill.Instruction.Instruction = InstructionDrawer.Draw(instructionRect, null, skill.Instruction.Instruction, GUIContent.none);

				if (changes.changed && skill.Instruction.Instruction != null)
					RefreshInputs(inputs, skill.Instruction);
			}

			for (var i = 0; i < skill.Instruction.Inputs.Count; i++)
			{
				var input = skill.Instruction.Inputs[i];
				var inputRect = EditorGUI.PrefixLabel(inputsRect, new GUIContent(input.Name));
				VariableValueDrawer.DrawValue(inputRect, input, inputs.Inputs[i].Type, inputs.Inputs[i].ParentType);
				inputsRect.y += EditorGUIUtility.singleLineHeight + 5;
			}
		}

		private VariableType GetVariableType(Type type)
		{
			if (type == typeof(bool)) return VariableType.Boolean;
			else if (type == typeof(int)) return VariableType.Integer;
			else if (type == typeof(float)) return VariableType.Number;
			else if (type == typeof(string)) return VariableType.String;
			else if (typeof(ScriptableObject).IsAssignableFrom(type)) return VariableType.Asset;
			//else if (type == typeof(GameObject)) return VariableType.GameObject;
			else return VariableType.Other;
		}

		private void RefreshInputs(InstructionInputList inputs, SkillInstruction caller)
		{
			inputs.Inputs.Clear();

			if (caller.Instruction != null)
			{
				caller.Instruction.FindInputs(inputs, VariableLocation.Context);

				while (caller.Inputs.Count < inputs.Inputs.Count)
					caller.Inputs.Add(VariableValue.CreateEmpty(""));

				if (caller.Inputs.Count > inputs.Inputs.Count)
					caller.Inputs.RemoveRange(inputs.Inputs.Count, caller.Inputs.Count - inputs.Inputs.Count);

				for (var i = 0; i < inputs.Inputs.Count; i++)
				{
					var input = caller.Inputs[i];
					var variable = inputs.Inputs[i];
					var type = GetVariableType(variable.Type);

					if (input.Name != variable.Variable.Name || input.Type != type)
					{
						VariableValue.Destroy(input);
						caller.Inputs[i] = VariableValue.Create(variable.Variable.Name, type);
					}
				}
			}
		}

		public void Draw()
		{
			_skills.DrawList();
		}
	}
}
