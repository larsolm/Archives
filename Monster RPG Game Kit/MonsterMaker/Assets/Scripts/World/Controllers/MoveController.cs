using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[RequireComponent(typeof(Mover))]
	public abstract class MoveController : MonoBehaviour
	{
		public bool IsFrozen { get { return _frozenCount > 0; } }
		public Mover Mover { get; private set; }

		public void Freeze()
		{
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

		private int _frozenCount = 0;

		private void Awake()
		{
			Mover = GetComponent<Mover>();
		}
	}
}
