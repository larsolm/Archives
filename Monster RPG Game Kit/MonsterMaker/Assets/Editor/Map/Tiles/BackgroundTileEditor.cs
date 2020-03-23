using System;
using UnityEngine;
using UnityEditor;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(BackgroundTile))]
	public class BackgroundTileEditor : Editor
	{
		private BackgroundTile _tile { get { return (target as BackgroundTile); } }

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			
			GUIStyle style = new GUIStyle();
			style.fontStyle = FontStyle.Bold;
			style.fontSize *= 2;
			
			_tile.NoiseScale = EditorGUILayout.Slider("Noise Scale", _tile.NoiseScale, 0.0f, 1.0f);
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Sprites", style);

			int length = EditorGUILayout.DelayedIntField("Frames", _tile.Sprites.Length);
			if (length != _tile.Sprites.Length)
				Array.Resize(ref _tile.Sprites, Math.Max(length, 1));

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			for (var i = 0; i < _tile.Sprites.Length; i++)
				_tile.Sprites[i] = EditorGUILayout.ObjectField(GUIContent.none, _tile.Sprites[i], typeof(Sprite), false, GUILayout.MaxWidth(75)) as Sprite;

			EditorGUILayout.EndHorizontal();

			if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(_tile);
		}
	}
}
