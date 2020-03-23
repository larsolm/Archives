using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	[CreateAssetMenu(fileName = "Default Background Tile", menuName = "Monster Maker/World/Background Tile")]
	public class BackgroundTile : TileBase
	{
		public float NoiseScale = 0.75f;

		[HideInInspector] public Sprite[] Sprites = new Sprite[1];

		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			base.GetTileData(position, tilemap, ref tileData);

			var noise = Mathf.FloorToInt(Mathf.PerlinNoise((position.x + 100000) * NoiseScale, (position.y + 100000) * NoiseScale) * Sprites.Length);
			var index = Mathf.Clamp(noise, 0, Sprites.Length - 1);

			tileData.sprite = Sprites[index];
		}
	}
}
