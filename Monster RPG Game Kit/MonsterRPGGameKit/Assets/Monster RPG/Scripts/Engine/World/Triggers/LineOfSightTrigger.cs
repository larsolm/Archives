using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[RequireComponent(typeof(Mover))]
	[RequireComponent(typeof(Interaction))]
	[AddComponentMenu("PiRho Soft/World/Line Of Sight Trigger")]
	[HelpURL(MonsterRpg.DocumentationUrl + "line-of-sight-trigger")]
	public class LineOfSightTrigger : MonoBehaviour
	{
		[Tooltip("The amount of tiles in forward before the interaction is triggered")]
		[Minimum(1)]
		public int Distance = 3;

		[Tooltip("Whether the trigger searches for the player through walls or not")]
		public bool SeesThroughWalls = false;

		private Mover _mover;
		private Interaction _interaction;
		private bool _spotted = false;

		void Start()
		{
			_interaction = GetComponent<Interaction>();
			_mover = GetComponent<Mover>();
			_mover.OnDirectionChanged += DirectionChanged;
		}

		void OnEnable()
		{
			if (Player.Instance)
				Player.Instance.Mover.OnTileChanged += PlayerMoved;
		}

		void OnDisable()
		{
			if (Player.Instance)
				Player.Instance.Mover.OnTileChanged -= PlayerMoved;
		}

		private void PlayerMoved(Vector2Int previous, Vector2Int current)
		{
			CheckSightline(current);
		}

		private void DirectionChanged(MovementDirection previous, MovementDirection current)
		{
			if (!Player.Instance.Mover.Moving)
				CheckSightline(Player.Instance.Mover.CurrentGridPosition);
		}

		private void CheckSightline(Vector2Int target)
		{
			if (!_spotted)
			{
				var direction = _mover.DirectionVector;
				var start = _mover.CurrentGridPosition;
				var end = start + (direction * Distance);

				if (SeesThroughWalls)
				{
					if ((target.x == start.x && ((target.y > start.y && target.y <= end.y) || (target.y < start.y && target.y >= end.y))) || (target.y == start.y && ((target.x > start.x && target.x <= end.x) || (target.x < start.x && target.x >= end.x))))
						Spotted(target);
				}
				else
				{
					var layer = _mover.MovementLayer;
					if (target.x == start.x || target.y == start.y)
					{
						for (var position = start + direction;; position += direction)
						{
							if (position == target)
							{
								Spotted(target);
								return;
							}

							if (WorldManager.Instance.IsOccupied(position, layer))
								return;

							var tile = WorldManager.Instance.FindTile(position);
							if (tile != null)
							{
								if (tile.LayerChange != CollisionLayer.None)
									layer = tile.LayerChange;

								if (tile.HasCollision(layer) || tile.IsEdge(_mover.MovementDirection) || WorldManager.Instance.IsOccupied(position, layer))
									return;
							}

							if (position == end)
								return;
						}
					}
				}
			}
		}

		private void Spotted(Vector2Int target)
		{
			_spotted = true;

			// Delay for a frame so that player movement doesn't continue into the possible path of a moving npc
			Player.Instance.Mover.SkipNextUpdate();
			Player.Instance.ForceInteract(_interaction);
		}
	}
}
