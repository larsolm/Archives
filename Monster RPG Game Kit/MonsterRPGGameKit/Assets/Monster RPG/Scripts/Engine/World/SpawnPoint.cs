using PiRhoSoft.CompositionEngine;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public struct SpawnPoint
	{
		public static SpawnPoint Default = new SpawnPoint { Name = null, Direction = MovementDirection.Down, Layer = CollisionLayer.One };

		public Vector2Int Position;

		[Tooltip("The name of this spawn point to be referenced by a zone trigger")]
		public string Name;

		[Tooltip("The transition to play when this spawn point is spawned at (if null, DefaultSpawnTransition on the World will be used)")]
		public Transition Transition;

		[Tooltip("The direction the mover will face when spawning at this spawn point")]
		public MovementDirection Direction;

		[Tooltip("The collision layer to set the mover to when they spawn at this spawn point")]
		public CollisionLayer Layer;

		[Tooltip("Whether the mover should move one tile in the direction this spawn is facing when it spawns")]
		public bool Move;

		public bool IsNamed => !string.IsNullOrEmpty(Name);
	}
}
