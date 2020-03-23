using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[RequireComponent(typeof(Renderer))]
	[HelpURL(MonsterRpg.DocumentationUrl + "static-collider")]
	[AddComponentMenu("PiRho Soft/World/Static Collider")]
	public class StaticCollider : MonoBehaviour
	{
		[Tooltip("The collision layer this collider will sort on")]
		public CollisionLayer CollisionLayer = CollisionLayer.One;

		private RectInt _occupiedTiles;

		void Awake()
		{
			var renderer = GetComponent<Renderer>();
			var collider = GetComponent<Collider2D>();

			var bounds = collider ? collider.bounds : renderer.bounds;

			var min = Vector2Int.FloorToInt(bounds.min);
			var max = Vector2Int.CeilToInt(bounds.max);

			_occupiedTiles = new RectInt(min, max - min);
			renderer.sortingOrder = LayerSorting.GetSortingOrder(CollisionLayer);
		}

		void OnEnable()
		{
			OccupyCurrentTiles();
		}

		void OnDisable()
		{
			UnoccupyCurrentTiles();
		}

		public void OccupyCurrentTiles()
		{
			foreach (var position in _occupiedTiles.allPositionsWithin)
			{
				WorldManager.Instance?.SetOccupied(position, CollisionLayer);
				WorldManager.Instance?.AddInteraction(position, GetComponent<Interaction>());
			}
		}

		public void UnoccupyCurrentTiles()
		{
			foreach (var position in _occupiedTiles.allPositionsWithin)
			{
				WorldManager.Instance?.SetUnoccupied(position, CollisionLayer);
				WorldManager.Instance?.RemoveInteraction(position, GetComponent<Interaction>());
			}
		}
	}
}
