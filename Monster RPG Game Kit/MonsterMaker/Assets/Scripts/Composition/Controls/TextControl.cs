using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/UI/Text Control")]
	public class TextControl : MonoBehaviour
	{
		protected SelectionControl _selection;
		protected InstructionListener _listener;
		protected bool _finished;
		protected bool _accepted;

		public bool IsActivated { get { return _listener != null && isActiveAndEnabled; } }

		public void Activate(InstructionListener listener, string message)
		{
			Activate(listener, null, message);
		}

		public void Activate(InstructionListener listener, SelectionControl selection, string message)
		{
			_selection = selection;
			_listener = listener;
			_finished = false;

			SetupText(message);
		}

		public void Deactivate()
		{
			_listener = null;
		}

		public virtual void Accept()
		{
			_accepted = true;
		}

		public virtual void Cancel()
		{
			_listener.Dismiss();
		}

		protected virtual void SetupText(string message)
		{
		}

		protected virtual void Awake()
		{
			gameObject.SetActive(false);
		}

		protected virtual void Update()
		{
			if (_finished && _selection)
				_selection.gameObject.SetActive(true);
			else if (_accepted)
				Advance();

			_accepted = false;
		}

		protected virtual void Advance()
		{
			_finished = true;

			if (!_selection)
				_listener.Dismiss();
		}
	}
}
