using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[RequireComponent(typeof(StaticMover))]
	[AddComponentMenu("Monster Maker/World/Static Sorting")]
	public class StaticSorting : LayerSorting
	{
		protected override void Awake()
		{
			base.Awake();

			var mover = GetComponent<StaticMover>();
			UpdateSorting(mover.CollisionLayer, true);
		}
	}
}
