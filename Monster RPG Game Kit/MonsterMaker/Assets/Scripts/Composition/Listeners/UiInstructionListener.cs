using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Composition/Ui Instruction Listener")]
	public class UiInstructionListener : InstructionListener
	{
		[Tooltip("The object that will control regular text.")] public TextControl TextControl;
		[Tooltip("The object that will control selections.")] public SelectionControl SelectionControl;

		public override void ProcessInput()
		{
			if (Player.Instance.Left.Pressed)
				Left();
			else if (Player.Instance.Right.Pressed)
				Right();
			else if (Player.Instance.Up.Pressed)
				Up();
			else if (Player.Instance.Down.Pressed)
				Down();

			if (Player.Instance.Interact.Pressed)
				Accept();
			else if (Player.Instance.Cancel.Pressed)
				Cancel();
			else if (Player.Instance.Pause.Pressed)
				Pause();
		}

		public virtual void Accept()
		{
			if (TextControl)
			{
				if (TextControl.IsActivated)
					TextControl.Accept();
			}

			if (SelectionControl)
			{
				if (SelectionControl.IsActivated)
					SelectionControl.Accept();
			}
		}

		public virtual void Cancel()
		{
			if (TextControl)
			{
				if (TextControl.IsActivated)
					TextControl.Cancel();
			}

			if (SelectionControl)
			{
				if (SelectionControl.IsActivated)
					SelectionControl.Cancel();
			}
		}

		public virtual void Pause()
		{
			Dismiss();
		}

		public virtual void Left()
		{
			if (SelectionControl)
			{
				if (SelectionControl.IsActivated)
					SelectionControl.Left();
			}
		}

		public virtual void Right()
		{
			if (SelectionControl)
			{
				if (SelectionControl.IsActivated)
					SelectionControl.Right();
			}
		}

		public virtual void Up()
		{
			if (SelectionControl)
			{
				if (SelectionControl.IsActivated)
					SelectionControl.Up();
			}
		}

		public virtual void Down()
		{
			if (SelectionControl)
			{
				if (SelectionControl.IsActivated)
					SelectionControl.Down();
			}
		}

		protected override void OnPrompt(PromptInstruction instruction, object data, InstructionContext context)
		{
			var message = instruction as MessagePrompt;
			if (message)
			{
				if (TextControl)
				{
					TextControl.Activate(this, message.Message.GetString(context));
					TextControl.gameObject.SetActive(true);
				}
				else
				{
					Debug.LogFormat("UIDialogListener for catagory {0} must have a TextControl to respond to messages.", Category);
				}
			}

			var selection = instruction as SelectionInstruction;
			if (selection)
			{
				if (SelectionControl)
				{
					SelectionControl.Activate(this, data as object[]);

					if (TextControl)
					{
						TextControl.Activate(this, SelectionControl, selection.Message.GetString(context));
						TextControl.gameObject.SetActive(true);
					}
					else
					{
						Debug.LogFormat("UIDialogListener for catagory {0} must have a TextControl to respond to selections with messages.", Category);
					}
				}
				else
				{
					Debug.LogFormat("UIDialogListener for catagory {0} must have a SelectionControl to respond to selections.", Category);
				}
			}
		}

		protected override void OnDismissed(InstructionContext context)
		{
			if (TextControl && TextControl.IsActivated)
			{
				TextControl.Deactivate();
				TextControl.gameObject.SetActive(false);
			}

			if (SelectionControl && SelectionControl.IsActivated)
			{
				SelectionControl.Deactivate();
				SelectionControl.gameObject.SetActive(false);
			}
		}
	}
}
