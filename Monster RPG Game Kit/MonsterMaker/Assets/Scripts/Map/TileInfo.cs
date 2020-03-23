using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class TileInfo
	{
		public Vector2Int Position;
		
		public CollisionLayer CollisionLayer = CollisionLayer.None;
		public CollisionLayer OccupiedLayer = CollisionLayer.None;
		public CollisionLayer CollisionLayerIncrement = CollisionLayer.None;

		public bool HasSpawnPoint = false;
		public SpawnPoint SpawnPoint;

		public bool HasZoneTrigger = false;
		public ZoneTrigger Zone;

		public bool HasEncounter = false;
		public Encounter Encounter;

		public bool HasInstructions = false;
		public Instruction Instructions;

		public bool HasCollision(CollisionLayer layer)
		{
			return (CollisionLayer & layer) > 0;
		}

		public bool IsOccupied(CollisionLayer layer)
		{
			return HasCollision(layer) || (OccupiedLayer & layer) > 0 || (OccupiedLayer & CollisionLayerIncrement) > 0;
		}

		public bool IsOccupiedOrHasCollision(CollisionLayer layer)
		{
			return HasCollision(layer) || IsOccupied(layer);
		}

		public bool IsEmpty()
		{
			return CollisionLayer == CollisionLayer.None && OccupiedLayer == CollisionLayer.None && CollisionLayerIncrement == CollisionLayer.None && !HasSpawnPoint && !HasZoneTrigger && !HasEncounter && !HasInstructions;
		}

		public bool IsSameCollisionLayer(TileInfo other)
		{
			if (other == null)
				return false;

			return CollisionLayer == other.CollisionLayer;
		}
		
		public bool IsSameOccupiedLayer(TileInfo other)
		{
			if (other == null)
				return false;

			return OccupiedLayer == other.OccupiedLayer;
		}

		public bool IsSameCollisionLayerIncrement(TileInfo other)
		{
			if (other == null)
				return false;

			return CollisionLayerIncrement == other.CollisionLayerIncrement;
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
	}
}
