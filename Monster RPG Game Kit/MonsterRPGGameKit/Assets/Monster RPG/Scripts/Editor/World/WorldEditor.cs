using System.Collections;
using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	[CustomEditor(typeof(World))]
	public class WorldEditor : Editor
	{
		private static readonly IconButton _editZoneButton = new IconButton(IconButton.Edit, "Edit this Zone");
		private static readonly IconButton _addSceneButton = new IconButton(IconButton.Add, "Edit this Zone");
		private static readonly IconButton _removeSceneButton = new IconButton(IconButton.Remove, "Edit this Zone");
		private static readonly Label _zonesLabel = new Label(typeof(World), nameof(World.Zones));
		private static readonly Label _uiScenesLabel = new Label(typeof(World), nameof(World.UiScenes));

		private World _world;

		private SerializedProperty _mainSceneProperty;
		private SerializedProperty _defaultZoneTransitionProperty;
		private SerializedProperty _defaultSpawnTransitionProperty;
		private SerializedProperty _backgroundMusicProperty;
		private SerializedProperty _worldSchemaProperty;
		private SerializedProperty _playerSchemaProperty;
		private SerializedProperty _npcSchemaProperty;
		private SerializedProperty _mapLayersProperty;
		private PropertyListControl _uiScenesControl = new PropertyListControl();
		private ObjectListControl _zonesControl = new ObjectListControl();

		void OnEnable()
		{
			_world = target as World;

			_mainSceneProperty = serializedObject.FindProperty(nameof(World.MainScene));
			_defaultZoneTransitionProperty = serializedObject.FindProperty(nameof(World.DefaultZoneTransition));
			_defaultSpawnTransitionProperty = serializedObject.FindProperty(nameof(World.DefaultSpawnTransition));
			_backgroundMusicProperty = serializedObject.FindProperty(nameof(World.BackgroundMusic));
			_worldSchemaProperty = serializedObject.FindProperty(nameof(World.WorldSchema));
			_playerSchemaProperty = serializedObject.FindProperty(nameof(World.PlayerSchema));
			_npcSchemaProperty = serializedObject.FindProperty(nameof(World.NpcSchema));
			_mapLayersProperty = serializedObject.FindProperty(nameof(World.MapLayers));

			_uiScenesControl.Setup(serializedObject.FindProperty(nameof(World.UiScenes)))
				.MakeAddable(_addSceneButton)
				.MakeRemovable(_removeSceneButton)
				.MakeReorderable()
				.MakeCollapsable("MonsterRpgGameKit.WorldEditor.ScenesOpen");

			_zonesControl.Setup(_world.Zones)
				.MakeDrawable(DrawZone)
				.MakeReorderable()
				.MakeCollapsable("MonsterRpgGameKit.WorldEditor.ZonesOpen");
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.PropertyField(_mainSceneProperty);
				EditorGUILayout.PropertyField(_defaultZoneTransitionProperty);
				EditorGUILayout.PropertyField(_defaultSpawnTransitionProperty);
				EditorGUILayout.PropertyField(_backgroundMusicProperty);
				EditorGUILayout.PropertyField(_worldSchemaProperty);
				EditorGUILayout.PropertyField(_playerSchemaProperty);
				EditorGUILayout.PropertyField(_npcSchemaProperty);
				EditorGUILayout.PropertyField(_mapLayersProperty);

				_uiScenesControl.Draw(_uiScenesLabel.Content);
			}

			using (new UndoScope(_world, false))
				_zonesControl.Draw(_zonesLabel.Content);
		}

		private void DrawZone(Rect rect, IList list, int index)
		{
			var nameRect = new Rect(rect.x, rect.y, rect.width - EditorGUIUtility.singleLineHeight, rect.height);
			var editRect = new Rect(rect.xMax - EditorGUIUtility.singleLineHeight, rect.y, EditorGUIUtility.singleLineHeight, rect.height);
			var zone = _world.Zones[index];

			if (zone != null)
			{
				EditorGUI.LabelField(nameRect, zone.Name);
				if (GUI.Button(editRect, _editZoneButton.Content, GUIStyle.none))
					Selection.activeObject = zone;
			}
			else
			{
				EditorGUI.LabelField(nameRect, "(deleted)");
			}
		}
	}
}
