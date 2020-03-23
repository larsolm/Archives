using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[RequireComponent(typeof(Mover))]
	public abstract class Controller : MonoBehaviour
	{
		private int _frozenCount = 0;
		public Mover Mover { get; private set; }

		public bool IsFrozen { get { return _frozenCount > 0; } }

		protected virtual void Awake()
		{
			Mover = GetComponent<Mover>();
		}

		public void Freeze()
		{
			if (_frozenCount == 0)
				Mover.SkipNextUpdate();

			_frozenCount++;
		}

		public void Thaw()
		{
			_frozenCount--;
		}

		protected void UpdateMover(float horizontal, float vertical)
		{
			if (_frozenCount > 0)
				Mover.UpdateMove(0.0f, 0.0f);
			else
				Mover.UpdateMove(horizontal, vertical);
		}

		internal abstract void Load(string saveData);
		internal abstract string Save();
	}
}
