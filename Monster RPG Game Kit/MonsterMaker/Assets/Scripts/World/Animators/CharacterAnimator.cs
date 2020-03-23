using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[RequireComponent(typeof(Mover))]
	[RequireComponent(typeof(Animator))]
	[AddComponentMenu("Monster Maker/Animation/Character Animator")]
	class CharacterAnimator : MonoBehaviour
	{
		private Animator _animator;
		private Mover _mover;

		private void Awake()
		{
			_mover = GetComponent<Mover>();
			_animator = GetComponent<Animator>();
		}

		private void Update()
		{
			var speed = _mover.Speed;
			var direction = _mover.MoveDirection;

			_animator.SetFloat("Horizontal", direction.x);
			_animator.SetFloat("Vertical", direction.y);
			_animator.SetFloat("Speed", _mover.Speed / _mover.MoveSpeed);
		}
	}
}
