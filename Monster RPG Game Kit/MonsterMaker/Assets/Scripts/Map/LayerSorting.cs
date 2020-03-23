using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[RequireComponent(typeof(Renderer))]
	public abstract class LayerSorting : MonoBehaviour
	{
		private Renderer _renderer;

		protected virtual void Awake()
		{
			_renderer = GetComponent<Renderer>();
		}

		protected virtual void UpdateSorting(CollisionLayer layer, bool usePosition)
		{
			var index = GetLowestLayerIndex(layer) + 1;
			var sortingOrder = index * 100000;

			if (usePosition)
				sortingOrder += (int)(50000 - (transform.position.y * 100));

			_renderer.sortingOrder = sortingOrder;
		}

		private int GetLowestLayerIndex(CollisionLayer layer)
		{
			var value = (int)layer;
			for (var i = 0; i < CollisionLayerData.LayerCount; i++)
			{
				if (((value >> i) & 1) != 0)
					return i;
			}

			return 0;
		}
	}
}
