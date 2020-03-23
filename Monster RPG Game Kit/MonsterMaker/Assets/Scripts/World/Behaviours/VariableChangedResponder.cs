using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class VariableChangedResponder : MonoBehaviour, IVariableListener
	{
		public VariableReference Variable;
		public InstructionCaller Instructions;

		private InstructionContext _context;
		private VariableStore _store;

		public void OnVariableAdded(VariableValue variable, object owner)
		{
			if (variable.Name == Variable.Name)
				Trigger(variable);
		}

		public void OnVariableChanged(VariableValue variable, object owner)
		{
			if (variable.Name == Variable.Name)
				Trigger(variable);
		}

		public void OnVariableRemoved(string name, object owner)
		{
			if (name == Variable.Name)
				Trigger(null);
		}

		private void Awake()
		{
			_context = new WorldInstructionContext(gameObject, Variable.Name + " Responder", null);
			_store = _context.GetStore(Variable);

			if (_store != null)
			{
				var variable = _store.GetVariable(Variable.Name);
				_store.Subscribe(this, null);
				Trigger(variable);
			}
		}

		private void OnDestroy()
		{
			if (_store != null)
				_store.Unsubscribe(this);
		}

		private void Trigger(VariableValue variable)
		{
			if (Instructions != null)
				InstructionManager.Instance.StartCoroutine(Instructions.Run(_context, this));
		}
	}
}
