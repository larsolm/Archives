using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterRpgEngine
{
	[HelpURL(MonsterRpg.DocumentationUrl + "random-tile")]
	[CreateAssetMenu(fileName = nameof(RandomTile), menuName = "PiRho Soft/Tiles/Random Tile", order = 107)]
	public class RandomTile : TileBase
	{
		[Tooltip("The minimum speed of this animation")]
		[Range(0.0f, 10.0f)]
		public float NoiseScale = 2.5f;

		[Tooltip("The list of tiles this tile will choose to display randomly")]
		public List<TileTransformInfo> Tiles = new List<TileTransformInfo>();

		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			var tile = Tiles.Count > 0 ? Tiles[GetIndex(position, Tiles.Count)] : Tiles.FirstOrDefault();

			tileData.transform = tile.GetTransform();
			tileData.color = Color.white;
			tileData.sprite = tile.Sprite;
			tileData.colliderType = Tile.ColliderType.None;
		}

		public int GetIndex(Vector3Int position, int length)
		{
			var noise = Mathf.FloorToInt(Mathf.PerlinNoise(position.x * NoiseScale, position.y * NoiseScale) * length);
			var index = Mathf.Clamp(noise, 0, length - 1);

			return index;
		}
	}
}
