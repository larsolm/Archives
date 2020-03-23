using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable] public class InstructionDictionary : SerializedDictionary<string, Instruction> { }

	public class InstructionInputList
	{
		public class InstructionInput
		{
			public VariableReference Variable;
			public bool IsPrimitive;
			public Type Type;
			public Type ParentType;
		}

		public List<InstructionInput> Inputs = new List<InstructionInput>();

		public void AddPrimitive(VariableReference variable)
		{
			Inputs.Add(new InstructionInput
			{
				Variable = variable,
				IsPrimitive = true
			});
		}

		public void Add(VariableReference variable, FieldInfo field)
		{
			var input = new InstructionInput { Variable = variable };
			var attributes = field != null ? field.GetCustomAttributes(typeof(VariableTypeAttribute), false) : null;
			var attribute = attributes != null && attributes.Length > 0 ? (attributes[0] as VariableTypeAttribute) : null;

			if (attribute != null)
			{
				input.Type = attribute.Type;
				input.ParentType = attribute.ParentType;
			}

			Inputs.Add(input);
		}
	}

	public abstract class Instruction : ScriptableObject
	{
		[InstructionBreadcrumbs] public InstructionSet Set;
		internal bool AllowMultipleInstances = false;
		internal bool IsRunning = false;

		public virtual void Begin(InstructionContext context) {}
		public virtual bool Execute(InstructionContext context) { return true; }
		public virtual void End(InstructionContext context) {}

		private void OnEnable()
		{
			IsRunning = false;
		}

		public void FindInputs(InstructionInputList inputs, VariableLocation location)
		{
			var seen = new List<Instruction>();
			FindInputs(this, inputs, seen, location);
		}

		private static void FindInputs(Instruction instruction, InstructionInputList inputs, List<Instruction> seen, VariableLocation location)
		{
			if (instruction == null || seen.Contains(instruction))
				return;

			seen.Add(instruction);

			var expression = instruction as ExpressionInstruction;
			if (expression != null)
			{
				foreach (var statement in expression.Statements)
					statement.FindInputs(inputs, location);
			}

			var variableFields = instruction.GetType().GetFields().Where(field => field.FieldType == typeof(VariableReference));
			var promptFields = instruction.GetType().GetFields().Where(field => field.FieldType == typeof(PromptString));
			var instructionFields = instruction.GetType().GetFields().Where(field => typeof(Instruction).IsAssignableFrom(field.FieldType));
			var instructionListFields = instruction.GetType().GetFields().Where(field => field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>) && typeof(Instruction).IsAssignableFrom(field.FieldType.GetGenericArguments()[0]));

			foreach (var field in variableFields)
			{
				var value = field.GetValue(instruction) as VariableReference;

				if (value != null && value.Location == location)
					inputs.Add(value, field);
			}

			foreach (var field in promptFields)
			{
				var value = field.GetValue(instruction) as PromptString;

				if (value != null)
				{
					foreach (var input in value.Input)
					{
						if (input != null && input.Location == location)
							inputs.Add(input, field);
					}
				}
			}

			foreach (var field in instructionFields)
			{
				var value = field.GetValue(instruction) as Instruction;
				FindInputs(value, inputs, seen, location);
			}

			foreach (var listField in instructionListFields)
			{
				var instructions = listField.GetValue(instruction) as IList<Instruction>;

				for (var i = 0; i < instructions.Count; i++)
				{
					var value = instructions[i] as Instruction;
					FindInputs(value, inputs, seen, location);
				}
			}
		}
	}
}
