using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	[CustomEditor(typeof(Zone))]
	public class ZoneEditor : Editor
	{
		public static readonly Label _worldContent = new Label(typeof(Zone), nameof(Zone.World));
		public static readonly Label _sceneContent = new Label(typeof(Zone), nameof(Zone.Scene));
		public static readonly Label _mapLayerContent = new Label(typeof(Zone), nameof(Zone.MapLayer));

		private Zone _zone;

		private SerializedProperty _nameProperty;
		private SerializedProperty _backgroundMusicProperty;
		private SerializedProperty _enterInstructionsProperty;
		private SerializedProperty _exitInstructionsProperty;
		private SerializedProperty _schemaProperty;

		private GUIContent[] _mapLayers;

		void OnEnable()
		{
			_zone = target as Zone;

			_nameProperty = serializedObject.FindProperty(nameof(Zone.Name));
			_backgroundMusicProperty = serializedObject.FindProperty(nameof(Zone.BackgroundMusic));
			_enterInstructionsProperty = serializedObject.FindProperty(nameof(Zone.EnterInstructions));
			_exitInstructionsProperty = serializedObject.FindProperty(nameof(Zone.ExitInstructions));
			_schemaProperty = serializedObject.FindProperty(nameof(Zone.Schema));

			if (_zone.World)
				_mapLayers = _zone.World.MapLayers.Select(layer => new GUIContent(layer)).ToArray();
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(_zone, false))
			{
				var world = AssetPopupDrawer.Draw(_worldContent.Content, _zone.World, true, true, true);

				if (_zone.World != world)
				{
					if (_zone.World != null)
					{
						_zone.World.Zones.Remove(_zone);
						EditorUtility.SetDirty(_zone.World);
					}

					_zone.World = world;

					if (_zone.World != null)
					{
						if (string.IsNullOrEmpty(_zone.Name))
							_zone.Name = _zone.name;

						_zone.World.Zones.Add(_zone);
						EditorUtility.SetDirty(_zone.World);

						_mapLayers = _zone.World.MapLayers.Select(layer => new GUIContent(layer)).ToArray();
					}
				}
			}

			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.PropertyField(_nameProperty);
			}

			using (new UndoScope(_zone, false))
			{
				SceneReferenceDrawer.Draw(_zone.Scene, _sceneContent.Content, _zone.Name, () =>
				{
					var map = new GameObject("Map");
					map.isStatic = true;
					map.AddComponent<Grid>();
					map.AddComponent<MapProperties>();
				});

				if (_zone.World != null)
				{
					var index = _zone.World.MapLayers.IndexOf(_zone.MapLayer);
					index = EditorGUILayout.Popup(_mapLayerContent.Content, index, _mapLayers);

					if (index >= 0)
						_zone.MapLayer = _zone.World.MapLayers[index];
				}
				else
				{
					EditorGUILayout.LabelField(_mapLayerContent.Content);
				}
			}

			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.PropertyField(_backgroundMusicProperty);
				EditorGUILayout.PropertyField(_enterInstructionsProperty);
				EditorGUILayout.PropertyField(_exitInstructionsProperty);
				EditorGUILayout.PropertyField(_schemaProperty);
			}
		}
	}
}