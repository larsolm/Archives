using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Mover))]
	[RequireComponent(typeof(Animator))]
	[HelpURL(MonsterRpg.DocumentationUrl + "mover-animator")]
	[AddComponentMenu("PiRho Soft/Animations/Mover Animator")]
	public class MoverAnimator : MonoBehaviour
	{
		private Animator _animator;
		private Mover _mover;

		private int _horizontal = Animator.StringToHash("Horizontal");
		private int _vertical = Animator.StringToHash("Vertical");
		private int _speed = Animator.StringToHash("Speed");

		void Awake()
		{
			_mover = GetComponent<Mover>();
			_animator = GetComponent<Animator>();
		}

		void Update()
		{
			var direction = _mover.DirectionVector;

			_animator.SetFloat(_horizontal, Mathf.Clamp(direction.x, -1, 1));
			_animator.SetFloat(_vertical, Mathf.Clamp(direction.y, -1, 1));
			_animator.SetFloat(_speed, Mathf.Clamp01(_mover.Speed));
		}
	}
}
