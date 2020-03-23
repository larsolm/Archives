using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class PromptString
	{
		[TextArea(minLines: 5, maxLines: 10)] public string Message;
		public VariableReference[] Input;

		public string GetString(InstructionContext context)
		{
			if (string.IsNullOrEmpty(Message))
			{
				return "";
			}
			else if (Input != null && Input.Length > 0)
			{
				if (_parameters == null || _parameters.Length != Input.Length)
					_parameters = new string[Input.Length];

				for (var i = 0; i < Input.Length; i++)
					_parameters[i] = GetVariableString(Input[i], context);

				try
				{
					return string.Format(Message, _parameters);
				}
				catch
				{
					Debug.Log("PromptString has the wrong number of inputs", context.Owner);
				}
			}

			return Message;
		}

		private string[] _parameters;

		private string GetVariableString(VariableReference variable, InstructionContext context)
		{
			var store = context.GetStore(variable);

			if (store != null)
			{
				var value = store.GetVariable(variable.Name);
				if (value != null)
					return value.ToString();
			}

			return "";
		}
	}

	public class PromptInstruction : Instruction
	{
		[ListenerCategory]
		[Tooltip("The catagory of the PromptListener that will respond to this instruction")]
		public string Category;

		public virtual void OnIgnored(InstructionContext context)
		{
		}

		public virtual void OnDismissed(InstructionContext context)
		{
		}

		public virtual void OnSelected(InstructionContext context, object selection)
		{
		}
	}
}
