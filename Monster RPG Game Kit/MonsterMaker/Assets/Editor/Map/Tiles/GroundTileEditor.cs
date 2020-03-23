using System;
using UnityEngine;
using UnityEditor;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(GroundTile))]
	public class GroundTileEditor : Editor
	{
		private GroundTile _tile { get { return (target as GroundTile); } }

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			for (var i = 0; i < _tile.TileInfo.Length; i++)
				DrawTileInfo(_tile.TileInfo[i], ((GroundTile.SpriteType)i).ToString());

			if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(_tile);
		}

		private void DrawTileInfo(GroundTile.Info info, string label)
		{
			GUIStyle style = new GUIStyle();
			style.fontStyle = FontStyle.Bold;
			style.fontSize *= 2;
			
			EditorGUILayout.LabelField(label, style);
			EditorGUILayout.BeginHorizontal();

			if (info.SpriteType == GroundTile.Info.Type.Static)
				info.Sprites[0] = EditorGUILayout.ObjectField(GUIContent.none, info.GetSprite(Vector3Int.zero), typeof(Sprite), false, GUILayout.MaxWidth(75)) as Sprite;

			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();

			info.SpriteType = (GroundTile.Info.Type)EditorGUILayout.EnumPopup("Sprite Type", info.SpriteType);
			info.Rotation = EditorGUILayout.IntPopup("Rotation", info.Rotation, new string[] { "0", "90", "180", "270" }, new int[] { 0, 90, 180, 270 });
			info.FlippedHorizontally = EditorGUILayout.Toggle("Flipped Horizontally", info.FlippedHorizontally);
			info.FlippedVertically = EditorGUILayout.Toggle("Flipped Vertically", info.FlippedVertically);

			if (info.SpriteType != GroundTile.Info.Type.Static)
			{
				if (info.SpriteType == GroundTile.Info.Type.Animated)
					info.AnimationSpeed = EditorGUILayout.FloatField("Animation Speed", info.AnimationSpeed);

				if (info.SpriteType == GroundTile.Info.Type.Random)
					info.NoiseScale = EditorGUILayout.Slider("Noise Scale", info.NoiseScale, 0.0f, 1.0f);

				int length = EditorGUILayout.DelayedIntField("Frames", info.Sprites.Length);
				if (length != info.Sprites.Length)
					Array.Resize(ref info.Sprites, Math.Max(length, 1));

				EditorGUILayout.BeginHorizontal();

				for (var i = 0; i < info.Sprites.Length; i++)
					info.Sprites[i] = EditorGUILayout.ObjectField(GUIContent.none, info.Sprites[i], typeof(Sprite), false, GUILayout.MaxWidth(75)) as Sprite;

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
	}
}
