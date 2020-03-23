using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterMaker
{
	[CreateAssetMenu(fileName = "Default Ground Tile", menuName = "Monster Maker/World/Ground Tile")]
	public class GroundTile : TileBase
	{
		[Serializable]
		public class Info
		{
			public enum Type
			{
				Static,
				Animated,
				Random
			}

			public Type SpriteType = Type.Static;
			public Sprite[] Sprites = new Sprite[1];
			public bool FlippedHorizontally = false;
			public bool FlippedVertically = false;
			public int Rotation;
			public float NoiseScale = 0.75f;
			public float AnimationSpeed = 1.0f;

			public Sprite GetSprite(Vector3Int position)
			{
				if (SpriteType == Type.Random)
				{
					var noise = Mathf.FloorToInt(Mathf.PerlinNoise((position.x + 100000) * NoiseScale, (position.y + 100000) * NoiseScale) * Sprites.Length);
					var index = Mathf.Clamp(noise, 0, Sprites.Length - 1);
					return Sprites[index];
				}

				return Sprites[0];
			}

			public Matrix4x4 GetTransform()
			{
				return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -(float)Rotation), new Vector3(FlippedHorizontally ? -1 : 1, FlippedVertically ? -1 : 1, 1));
			}
		}

		public enum SpriteType
		{
			TopLeft,
			Top,
			TopRight,
			Left,
			Center,
			Right,
			BottomLeft,
			Bottom,
			BottomRight,
			MiddleTopLeft,
			MiddleTopRight,
			MiddleBottomLeft,
			MiddleBottomRight,
			DiagonalForward,
			DiagonalBack,
			Count
		}

		[HideInInspector] public Info[] TileInfo = new Info[(int)SpriteType.Count];

		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			var info = GetTileInfo(tilemap, position);

			tileData.colliderType = Tile.ColliderType.None;
			tileData.flags = TileFlags.LockTransform;
			tileData.sprite = info.GetSprite(position);
			tileData.transform = info.GetTransform();
		}
		
		public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
		{
			var info = GetTileInfo(tilemap, position);

			if (info.SpriteType == Info.Type.Animated)
			{
				tileAnimationData.animatedSprites = info.Sprites;
				tileAnimationData.animationSpeed = info.AnimationSpeed;
			}

			return true;
		}

		private Info GetTileInfo(ITilemap tilemap, Vector3Int position)
		{
			var topLeft = HasNeighbor(tilemap, position, -1, 1);
			var top = HasNeighbor(tilemap, position, 0, 1);
			var topRight = HasNeighbor(tilemap, position, 1, 1);
			var left = HasNeighbor(tilemap, position, -1, 0);
			var right = HasNeighbor(tilemap, position, 1, 0);
			var bottomLeft = HasNeighbor(tilemap, position, -1, -1);
			var bottom = HasNeighbor(tilemap, position, 0, -1);
			var bottomRight = HasNeighbor(tilemap, position, 1, -1);

			if (TileInfo.Length != (int)SpriteType.Count)
				Array.Resize(ref TileInfo, (int)SpriteType.Count);

			if (topLeft && top && topRight && left && right && bottomLeft && bottom && bottomRight) return TileInfo[(int)SpriteType.Center];
			if (!topLeft && top && topRight && left && right && bottomLeft && bottom && bottomRight) return TileInfo[(int)SpriteType.MiddleTopLeft];
			if (topLeft && top && !topRight && left && right && bottomLeft && bottom && bottomRight) return TileInfo[(int)SpriteType.MiddleTopRight];
			if (topLeft && top && topRight && left && right && !bottomLeft && bottom && bottomRight) return TileInfo[(int)SpriteType.MiddleBottomLeft];
			if (topLeft && top && topRight && left && right && bottomLeft && bottom && !bottomRight) return TileInfo[(int)SpriteType.MiddleBottomRight];
			if (topLeft && top && !topRight && left && right && !bottomLeft && bottom && bottomRight) return TileInfo[(int)SpriteType.DiagonalForward];
			if (!topLeft && top && topRight && left && right && bottomLeft && bottom && !bottomRight) return TileInfo[(int)SpriteType.DiagonalBack];
			if (!top && !left && right && bottom) return TileInfo[(int)SpriteType.TopLeft];
			if (!top && left && right && bottom) return TileInfo[(int)SpriteType.Top];
			if (!top && left && !right && bottom) return TileInfo[(int)SpriteType.TopRight];
			if (top && !left && right && bottom) return TileInfo[(int)SpriteType.Left];
			if (top && left && !right && bottom) return TileInfo[(int)SpriteType.Right];
			if (top && !left && right && !bottom) return TileInfo[(int)SpriteType.BottomLeft];
			if (top && left && right && !bottom) return TileInfo[(int)SpriteType.Bottom];
			if (top && left && !right && !bottom) return TileInfo[(int)SpriteType.BottomRight];

			return TileInfo[(int)SpriteType.Center];
		}

		private bool HasNeighbor(ITilemap tilemap, Vector3Int position, int x, int y)
		{
			var tile = tilemap.GetTile<GroundTile>(position + new Vector3Int(x, y, 0));
			return tile != null && tile == this;
		}

		public override void RefreshTile(Vector3Int location, ITilemap tileMap)
		{
			for (var y = -1; y <= 1; y++)
			{
				for (var x = -1; x <= 1; x++)
					base.RefreshTile(location + new Vector3Int(x, y, 0), tileMap);
			}
		}
	}
}
