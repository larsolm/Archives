using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class InstructionCallerControl : PropertyControl
	{
		public InstructionCaller Caller;
		public SerializedProperty InstructionProperty;
		public SerializedProperty InputsProperty;
		public SerializedProperty OutputsProperty;
		public EditableList<InstructionVariable> InputsList;
		public EditableList<InstructionVariable> OutputsList;
		public InstructionInputList Inputs = new InstructionInputList();

		public override void Setup(SerializedProperty property, FieldInfo fieldInfo)
		{
			Caller = GetObject<InstructionCaller>(property);
			InstructionProperty = property.FindPropertyRelative("Instruction");
			InputsProperty = property.FindPropertyRelative("Inputs");
			OutputsProperty = property.FindPropertyRelative("Outputs");
			InputsList = new EditableList<InstructionVariable>();
			OutputsList = new EditableList<InstructionVariable>();

			var inputsList = InputsList.Setup(InputsProperty, null, null, false, true, false, true, true);
			OutputsList.Setup(OutputsProperty, null, null, false, true, false, true, true);

			inputsList.drawHeaderCallback += DrawRefreshButton;
		}

		public override float GetHeight(SerializedProperty property, GUIContent label)
		{
			var instructionHeight = EditorGUI.GetPropertyHeight(InstructionProperty);
			var inputsHeight = InputsList.Height;
			var outputsHeight = OutputsList.Height;

			return instructionHeight + 10 + inputsHeight + 10 + outputsHeight;
		}

		public override void Draw(Rect position, SerializedProperty property, GUIContent label)
		{
			var instructionHeight = EditorGUI.GetPropertyHeight(InstructionProperty);
			var inputsHeight = InputsList.Height;
			var outputsHeight = OutputsList.Height;

			var instructionRect = new Rect(position.x, position.y, position.width, instructionHeight);
			var inputsRect = new Rect(position.x, position.y + instructionHeight + 10, position.width, inputsHeight);
			var outputsRect = new Rect(position.x, position.y + instructionHeight + 10 + inputsHeight + 10, position.width, outputsHeight);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				EditorGUI.PropertyField(instructionRect, InstructionProperty);

				if (changes.changed && Caller.Instruction != null)
				{
					using (new EditObjectScope(property.serializedObject))
						RefreshInputs();
				}
			}

			InputsList.DrawList(inputsRect);
			OutputsList.DrawList(outputsRect);
		}

		private void AddInput(string name)
		{
			foreach (var input in Caller.Inputs)
			{
				if (name == input.Name)
					return;
			}

			Caller.Inputs.Add(new InstructionVariable { Name = name });
		}

		private void DrawRefreshButton(Rect rect)
		{
			var position = new Rect(rect.x + rect.width - 20, rect.y, 20, rect.height);
			var icon = EditorGUIUtility.IconContent("d_preAudioLoopOff");

			if (GUI.Button(position, icon, EditorStyles.toolbarButton))
				RefreshInputs();
		}

		private void RefreshInputs()
		{
			Inputs.Inputs.Clear();

			Caller.Instruction.FindInputs(Inputs, VariableLocation.Context);

			foreach (var input in Inputs.Inputs)
				AddInput(input.Variable.Name);
		}
	}

	[CustomPropertyDrawer(typeof(InstructionCaller))]
	public class InstructionCallerDrawer : ControlDrawer<InstructionCallerControl>
	{
	}
}
