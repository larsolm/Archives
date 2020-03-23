using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class InstructionListener : MonoBehaviour
	{
		[ListenerCategory] public string Category;

		public abstract void ProcessInput();

		public void Prompt(PromptInstruction instruction, object data, InstructionContext context)
		{
			Debug.Assert(_instruction == null && _data == null && _context == null, "a listener cannot receive multiple prompts at once", this);

			_instruction = instruction;
			_data = data;
			_context = context;

			OnPrompt(instruction, data, context);
		}
		
		public void Finish()
		{
			OnDismissed(_context);

			_instruction = null;
			_data = null;
			_context = null;
		}

		public void Dismiss()
		{
			_context.Dismiss(_instruction);
		}

		public void Select(object selection)
		{
			_context.Select(_instruction, selection);
		}

		protected abstract void OnPrompt(PromptInstruction instruction, object data, InstructionContext context);
		protected abstract void OnDismissed(InstructionContext context);

		private PromptInstruction _instruction;
		private object _data;
		private InstructionContext _context;

		private void OnEnable()
		{
			InstructionManager.Instance.AddListener(this);
		}

		private void OnDisable()
		{
			if (InstructionManager.Instance != null)
				InstructionManager.Instance.RemoveListener(this);
		}
	}
}
