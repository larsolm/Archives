using System;
using System.Collections.Generic;

namespace PiRhoSoft.MonsterMaker
{
	public class PriorityQueue<T> where T : IComparable<T>
	{
		public bool Empty { get { return _queue.Count == 0; } }

		private List<T> _queue = new List<T>();

		public void Push(T item)
		{
			_queue.Add(item);
			_queue.Sort();
		}

		public T Pop()
		{
			var last = _queue[_queue.Count - 1];
			_queue.RemoveAt(_queue.Count - 1);

			return last;
		}
	}
}
