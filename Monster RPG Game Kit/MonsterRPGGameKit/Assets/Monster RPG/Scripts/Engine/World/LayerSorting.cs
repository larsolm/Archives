using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Flags]
	public enum CollisionLayer
	{
		None = 0,
		One = 1,
		Two = 1 << 1,
		Three = 1 << 2,
		Four = 1 << 3,
		Five = 1 << 4,
		All = ~0
	}

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
	[HelpURL(MonsterRpg.DocumentationUrl + "layer-sorting")]
	[AddComponentMenu("PiRho Soft/World/Layer Sorting")]
	public class LayerSorting : MonoBehaviour
	{
		public static int LayerCount = Enum.GetValues(typeof(CollisionLayer)).Length - 2;
		private static int _bottomSortOffset = -100; // mover shadows are at -10, so this needs to be less than that

		[Tooltip("The movement layer this object will sort on")]
		public CollisionLayer Layer = CollisionLayer.One;

		[Tooltip("Set this to make this object sort below all other objects on the same layer (useful for tilemaps)")]
		public bool ForceToBottom = false;

		public static int GetSortingOrder(CollisionLayer layer)
		{
			return GetLowestLayerIndex(layer) * 1000;
		}

		private static int GetLowestLayerIndex(CollisionLayer layer)
		{
			var value = (int)layer;
			for (var i = 0; i < LayerCount; i++)
			{
				if (((value >> i) & 1) != 0)
					return i;
			}

			return 1;
		}

		void Awake()
		{
			var offset = ForceToBottom ? _bottomSortOffset : 0;
			var renderer = GetComponent<Renderer>();
			renderer.sortingOrder = GetSortingOrder(Layer) + offset;
		}
	}
}