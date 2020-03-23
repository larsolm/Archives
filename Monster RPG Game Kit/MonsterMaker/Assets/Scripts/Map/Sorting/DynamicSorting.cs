using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[RequireComponent(typeof(Mover))]
	[AddComponentMenu("Monster Maker/World/Dynamic Sorting")]
	public class DynamicSorting : LayerSorting
	{
		private Mover _mover;

		protected override void Awake()
		{
			base.Awake();

			_mover = GetComponent<Mover>();
		}

		private void LateUpdate()
		{
			UpdateSorting(_mover.CollisionLayer, true);
		}
	}
}