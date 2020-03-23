using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Composition/Log Instruction Listener")]
	public class LogInstructionListener : InstructionListener
	{
		private object[] _options = null;

		public override void ProcessInput()
		{
			if (Player.Instance.Interact.Pressed)
			{
				if (_options == null)
					Dismiss();
				else
					Select(_options[0]);
			}
		}

		protected override void OnPrompt(PromptInstruction instruction, object data, InstructionContext context)
		{
			var message = instruction as MessagePrompt;
			if (message)
				Debug.Log(message.Message);

			var selection = instruction as SelectionInstruction;
			if (selection)
			{
				_options = data as object[];
				Debug.Log(selection.Message);
				Debug.Log(_options);
			}
		}

		protected override void OnDismissed(InstructionContext context)
		{
			_options = null;
		}
	}
}
