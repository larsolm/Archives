using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class TileDictionary : SerializedDictionary<Vector2Int, TileInfo>
	{
	}

	[Serializable]
	public class TileInfo
	{
		public Vector2Int Position;
		
		[Tooltip("The collision layer(s) that this tile is a part of. Movers on these layers will not be able to enter this tile")]
		public CollisionLayer CollisionLayer = CollisionLayer.None;

		[Tooltip("The movement layer to set a mover to when they enter this tile")]
		public CollisionLayer LayerChange = CollisionLayer.None;

		[Tooltip("Whether this tile has a spawn point or not")]
		public bool HasSpawnPoint = false;
		public SpawnPoint SpawnPoint;

		[Tooltip("Whether this tile has a zone trigger or not")]
		public bool HasZoneTrigger = false;
		public ZoneTrigger Zone;

		[Tooltip("Whether this tile can trigger an encounter or not")]
		public bool HasEncounter = false;

		[Tooltip("The game object with the encounter to initiate when this tile is entered")]
		public Encounter Encounter;

		[Tooltip("Whether this tile should run instructions when entered or exited")]
		public bool HasInstructions = false;
		public InstructionTrigger Instructions;

		[Tooltip("Whether this tile has stairs that run right to left")]
		public bool HasStairs = false;
		[Tooltip("The slope of these stairs from left to right")]
		public int Slope = 1;

		[Tooltip("Whether this tile has a position to offset the mover to")]
		public bool HasOffset = false;

		[Tooltip("The position a mover should offset to when walking through this tile")]
		public Vector2 Offset = Vector2.zero;

		[Tooltip("Whether this tile has an edge that can only be passed over in one direction")]
		public bool HasEdge = false;

		[Tooltip("The direction this edge faces")]
		public MovementDirection EdgeDirection = MovementDirection.Down;

		public bool HasCollision(CollisionLayer layer)
		{
			return (CollisionLayer & layer) > 0;
		}

		public bool IsEdge(MovementDirection direction)
		{
			return HasEdge && EdgeDirection != direction;
		}

		public bool IsJumpable(MovementDirection direction)
		{
			return HasEdge && EdgeDirection == direction;
		}

		public bool IsEmpty()
		{
			return CollisionLayer == CollisionLayer.None && LayerChange == CollisionLayer.None && !HasSpawnPoint && !HasZoneTrigger && !HasEncounter && !HasInstructions && !HasStairs && !HasOffset && !HasEdge;
		}

		public bool IsSameCollisionLayer(TileInfo other)
		{
			if (other == null)
				return false;

			return CollisionLayer == other.CollisionLayer;
		}

		public bool IsSameCollisionLayerIncrement(TileInfo other)
		{
			if (other == null)
				return false;

			return LayerChange == other.LayerChange;
		}

		public bool IsSameZoneAs(TileInfo other)
		{
			if (other == null)
				return false;

			if (!HasZoneTrigger)
				return other.HasZoneTrigger;

			if (!other.HasZoneTrigger)
				return false;

			return other.Zone.TargetZone == Zone.TargetZone;
		}
		
		public bool IsSameEncounterAs(TileInfo other)
		{
			if (other == null)
				return false;

			if (!HasEncounter)
				return other.HasEncounter;

			if (!other.HasEncounter)
				return false;

			return other.Encounter == Encounter;
		}

		public bool IsSameInstructionAs(TileInfo other)
		{
			if (other == null)
				return false;
			
			if (!HasInstructions)
				return other.HasInstructions;

			if (!other.HasInstructions)
				return false;

			return other.Instructions == Instructions;
		}

		public bool IsSameStairsAs(TileInfo other)
		{
			if (other == null)
				return false;

			if (!HasStairs)
				return other.HasStairs;

			if (!other.HasStairs)
				return false;

			return other.Slope == Slope;
		}

		public bool IsSameOffsetAs(TileInfo other)
		{
			if (other == null)
				return false;

			if (!HasOffset)
				return other.HasOffset;

			if (!other.HasOffset)
				return false;

			return other.Offset == Offset;
		}

		public bool IsSameEdgeAs(TileInfo other)
		{
			if (other == null)
				return false;

			if (!HasEdge)
				return other.HasEdge;

			if (!other.HasEdge)
				return false;

			return other.EdgeDirection == EdgeDirection;
		}
	}
}