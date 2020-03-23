using System;
using System.Collections;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class SkillInstruction
	{
		public Instruction Instruction;
		public List<VariableValue> Inputs = new List<VariableValue>();

		public IEnumerator Run(InstructionContext context, UnityObject parent)
		{
			var frame = context.OpenFrame();
			WriteInputs(context);
			context.Run(Instruction, parent);
			yield return null;

			while (context.FrameCount > frame)
			{
				context.Update();
				yield return null;
			}

			context.CloseFrame();

			while (context.IsRunning)
			{
				context.Update();
				yield return null;
			}
		}

		private void WriteInputs(InstructionContext context)
		{
			foreach (var input in Inputs)
				context.Variables.Copy(input.Name, input);
		}
	}

	[Serializable]
	public class Skill
	{
		public string Name;
		public int LearnLimit = 1;
		public ExpressionStatement Condition = new ExpressionStatement();
		public SkillInstruction Instruction = new SkillInstruction();
		public List<string> Triggers = new List<string>();

		public void UpdateTriggers()
		{
			Triggers.Clear();

			var inputs = new InstructionInputList();
			Condition.FindInputs(inputs, VariableLocation.Custom);

			foreach (var input in inputs.Inputs)
			{
				if (input.Variable.CustomSource == "Creature")
					Triggers.Add(input.Variable.Name.Replace("Creature.", ""));
			}
		}
	}
}
