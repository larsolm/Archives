using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public struct SpawnPoint
	{
		public string Name;
		public Vector2 Direction;
		public Vector2Int Position;
		public CollisionLayer Layer;
	}
}
