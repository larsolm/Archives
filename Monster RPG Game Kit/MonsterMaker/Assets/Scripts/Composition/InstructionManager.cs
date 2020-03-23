using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Composition/Instruction Manager")]
	public class InstructionManager : Singleton<InstructionManager>
	{
		private Dictionary<string, List<InstructionListener>> _listeners = new Dictionary<string, List<InstructionListener>>();

		public void AddListener(InstructionListener listener)
		{
			GetOrAddListeners(listener.Category).Add(listener);
		}

		public void RemoveListener(InstructionListener listener)
		{
			var list = GetListeners(listener.Category);

			if (list == null || !list.Remove(listener))
				Debug.Log("attempted to remove a listener from the InstructionManager that has not been added", listener);
		}

		public List<InstructionListener> GetListeners(string category)
		{
			List<InstructionListener> listeners;
			return _listeners.TryGetValue(category, out listeners) ? listeners : null;
		}

		private List<InstructionListener> GetOrAddListeners(string category)
		{
			List<InstructionListener> listeners;

			if (!_listeners.TryGetValue(category, out listeners))
			{
				listeners = new List<InstructionListener>();
				_listeners.Add(category, listeners);
			}

			return listeners;
		}
	}
}
