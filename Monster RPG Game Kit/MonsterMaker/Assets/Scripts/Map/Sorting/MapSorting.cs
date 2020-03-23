using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[RequireComponent(typeof(CollisionLayerData))]
	[AddComponentMenu("Monster Maker/World/Map Sorting")]
	public class MapSorting : LayerSorting
	{
		protected override void Awake()
		{
			base.Awake();

			var collision = GetComponent<CollisionLayerData>();
			UpdateSorting(collision.CollisionLayer, false);
		}
	}
}
