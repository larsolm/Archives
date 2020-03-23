using System.Collections.Generic;

namespace PiRhoSoft.MonsterMaker
{
	public interface IPoolable
	{
		void Reset();
	}

	public class Pool<T> where T : IPoolable, new()
	{
		public Pool(int size, int growth)
		{
			_size = size;
			_growth = growth;
			_freeList = new Stack<T>(_size);

			for (var i = 0; i < size; i++)
				Release(new T());
		}

		public void Grow()
		{
			for (var i = 0; i < _growth; i++)
				Release(new T());
		}

		public T Reserve()
		{
			if (_freeList.Count == 0)
				Grow();

			return _freeList.Pop();
		}

		public void Release(T value)
		{
			value.Reset();
			_freeList.Push(value);
		}

		private int _size;
		private int _growth;
		private Stack<T> _freeList;
	}
}
