using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterMaker
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

	public class CollisionTile : TileBase
	{
		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			tileData.colliderType = Tile.ColliderType.Grid;
			tileData.sprite = null;
		}
	}
}
