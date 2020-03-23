using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(Zone))]
	public class ZoneEditor : Editor
	{
		private SerializedProperty _scene;
		private SerializedProperty _backgroundMusic;
		private SerializedProperty _clampBounds;
		private VariableStoreControl _staticState;
		private VariableStoreControl _defaultState;

		private GUIContent _mapLayer = new GUIContent("Map Layer", "The map layer that this zone is a part of.");

		private string[] _mapLayers;

		private void OnEnable()
		{
			var zone = target as Zone;

			_scene = serializedObject.FindProperty("Scene");
			_backgroundMusic = serializedObject.FindProperty("BackgroundMusic");
			_clampBounds = serializedObject.FindProperty("ClampBounds");
			_staticState = new VariableStoreControl(zone.StaticState, null, null, true, true);
			_defaultState = new VariableStoreControl(zone.DefaultState, null, null, true, true);

			_mapLayers = zone.World.MapLayers.ToArray();
		}

		public override void OnInspectorGUI()
		{
			var zone = target as Zone;
			var back = GUILayout.Button(EditorHelper.BackContent, GUILayout.Width(60.0f));

			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_clampBounds);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_scene);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_backgroundMusic);
				EditorGUILayout.Space();
			}

			using (new UndoScope(target))
			{
				var mapLayer = EditorGUILayout.Popup(_mapLayer, zone.World.MapLayers.IndexOf(zone.MapLayer), _mapLayers);
				if (mapLayer >= 0)
					zone.MapLayer = zone.World.MapLayers[mapLayer];

				EditorGUILayout.Space();
				_staticState.Draw();
				EditorGUILayout.Space();
				_defaultState.Draw();
			}

			if (back)
				EditorHelper.Edit(zone.World);
		}
	}
}
