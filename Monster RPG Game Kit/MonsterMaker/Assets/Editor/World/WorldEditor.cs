using System;
using System.Linq;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(World))]
	public class WorldEditor : Editor
	{
		private class AddZonePopup : PopupWindowContent
		{
			public AddZonePopup(WorldEditor editor)
			{
				_editor = editor;
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, EditorGUIUtility.singleLineHeight * 4);
			}

			public override void OnGUI(Rect rect)
			{
				EditorGUILayout.LabelField(_label);

				var enter = GuiFields.TextEnterField("NewZoneName", GUIContent.none, ref _newName);
				var create = GUILayout.Button(EditorHelper.CreateContent);

				if ((enter || create) && !string.IsNullOrEmpty(_newName))
				{
					_editor._toAdd = _newName;
					editorWindow.Close();
				}
			}
			
			private GUIContent _label = new GUIContent("New Zone", "Create new zone.");
			private WorldEditor _editor;
			private string _newName = "Name";
		}

		private SerializedProperty _backgroundMusic;
		
		private EditableList<SceneReference> _uiScenes = new EditableList<SceneReference>();
		private EditableList<Zone> _zones = new EditableList<Zone>();
		private EditableList<string> _mapLayers = new EditableList<string>();

		private string _toAdd = null;
		private Zone _toEdit = null;
		private Zone _toRemove = null;
		private int _toShow = -1;
		private int _toHide = -1;

		private GUIContent _mapLayer = new GUIContent("", "The name of this map layer.");
		private GUIContent _showLayer = new GUIContent("Show", "Load all the zones with this map layer.");
		private GUIContent _hideLayer = new GUIContent("Hide", "Unload all the zones with this map layer.");

		private void OnEnable()
		{
			_backgroundMusic = serializedObject.FindProperty("BackgroundMusic");
			_uiScenes.Setup(serializedObject.FindProperty("UIScenes"), null, null, false, true, false, true, true);
			_mapLayers.Setup(serializedObject.FindProperty("MapLayers"), null, null, true, true, false, true, true, DrawMapLayer);

			var zones = _zones.Setup(serializedObject.FindProperty("Zones"), null, null, true, true, false, true, true, DrawZone, RemoveZone);
			zones.onAddDropdownCallback += AddDropdown;

			_showLayer.image = EditorGUIUtility.IconContent("ViewToolOrbit On").image;
			_hideLayer.image = EditorGUIUtility.IconContent("ViewToolOrbit").image;
		}

		private void DrawMapLayer(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			var nameRect = new Rect(rect.x, rect.y, rect.width * 0.6f, EditorGUIUtility.singleLineHeight);
			var showRect = new Rect(nameRect.xMax + 5, rect.y, rect.width * 0.2f - 5, nameRect.height);
			var hideRect = new Rect(showRect.xMax + 5, rect.y, showRect.width, nameRect.height);

			EditorGUI.PropertyField(nameRect, element, _mapLayer);

			if (GUI.Button(showRect, _showLayer))
				_toShow = index;

			if (GUI.Button(hideRect, _hideLayer))
				_toHide = index;
		}

		private void DrawZone(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			var zone = element.objectReferenceValue as Zone;
			var nameRect = new Rect(rect.x, rect.y, rect.width * 0.6f, EditorGUIUtility.singleLineHeight);
			var editRect = new Rect(nameRect.xMax + 5, rect.y, rect.width * 0.2f - 5, nameRect.height);
			var loadRect = new Rect(editRect.xMax + 5, rect.y, editRect.width, nameRect.height);

			using (new UndoScope(zone))
				zone.name = EditorGUI.DelayedTextField(nameRect, zone.name);

			if (GUI.Button(editRect, EditorHelper.EditContent))
				_toEdit = zone;

			var loaded = zone.Scene.Scene.isLoaded;
			if (GUI.Button(loadRect, loaded ? EditorHelper.UnloadContent : EditorHelper.LoadContent))
			{
				if (loaded)
					EditorSceneManager.CloseScene(zone.Scene.Scene, true);
				else
					EditorSceneManager.OpenScene(zone.Scene.Path, OpenSceneMode.Additive);
			}
		}

		private void RemoveZone(SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			_toRemove = element.objectReferenceValue as Zone;
		}

		private void AddDropdown(Rect rect, ReorderableList list)
		{
			rect.y += EditorGUIUtility.singleLineHeight;
			PopupWindow.Show(rect, new AddZonePopup(this));
		}

		public override void OnInspectorGUI()
		{
			var world = target as World;

			_toAdd = null;
			_toRemove = null;
			_toEdit = null;
			_toShow = -1;
			_toHide = -1;

			using (new UndoScope(serializedObject))
				EditorGUILayout.PropertyField(_backgroundMusic);

			EditorGUILayout.Space();

			using (new UndoScope(serializedObject))
			{
				_uiScenes.DrawList();

				EditorGUILayout.Space();

				_mapLayers.DrawList();

				EditorGUILayout.Space();
				
				_zones.DrawList();
			}

			if (!string.IsNullOrEmpty(_toAdd))
				Add(world, _toAdd);

			if (_toRemove)
				Remove(world, _toRemove);

			if (_toShow >= 0)
				ShowLayer(_toShow);

			if (_toHide >= 0)
				HideLayer(_toHide);

			if (_toEdit)
				EditorHelper.Edit(_toEdit);
		}

		public void Add(World world, string name)
		{
			var existingNames = world.Zones.Select(i => i.name).ToArray();
			var zone = CreateInstance<Zone>();
			var newName = ObjectNames.GetUniqueName(existingNames, name);
			zone.World = world;
			zone.hideFlags = HideFlags.HideInHierarchy;
			zone.name = newName;
			zone.Scene.Path = CreateScene(newName);

			using (new UndoScope(world))
			{
				world.Zones.Add(zone);

				Undo.RegisterCreatedObjectUndo(zone, "Undo create zone");
				AssetDatabase.AddObjectToAsset(zone, world);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(world));
			}
		}

		private void Remove(World world, Zone zone)
		{
			using (new UndoScope(world))
			{
				world.Zones.Remove(zone);

				Undo.DestroyObjectImmediate(zone);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(world));
			}
		}

		private string CreateScene(string name)
		{
			var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
			SceneManager.SetActiveScene(scene);

			var map = new GameObject("Map");
			map.isStatic = true;
			map.AddComponent<Grid>();
			map.AddComponent<MapProperties>();

			EditorSceneManager.SaveScene(scene, "Assets/" + name + ".unity");
			SceneManager.UnloadSceneAsync(scene);

			var original = EditorBuildSettings.scenes;
			var newSettings = new EditorBuildSettingsScene[original.Length + 1];
			var sceneToAdd = new EditorBuildSettingsScene(scene.path, true);

			Array.Copy(original, newSettings, original.Length);

			newSettings[newSettings.Length - 1] = sceneToAdd;

			EditorBuildSettings.scenes = newSettings;

			return scene.path;
		}

		private void ShowLayer(int index)
		{
			var world = target as World;
			var layer = world.MapLayers[index];

			foreach (var zone in world.Zones)
			{
				if (zone.MapLayer == layer)
				{
					if (!zone.Scene.Scene.isLoaded)
						EditorSceneManager.OpenScene(zone.Scene.Path, OpenSceneMode.Additive);
				}
			}
		}

		private void HideLayer(int index)
		{
			var world = target as World;
			var layer = world.MapLayers[index];

			var scene = SceneManager.GetActiveScene();
			var activeZone = world.GetZoneBySceneIndex(scene.buildIndex);

			foreach (var zone in world.Zones)
			{
				if (zone.MapLayer == layer)
				{
					if (zone.Scene.Scene.isLoaded)
					{
						if (EditorSceneManager.loadedSceneCount == 1)
							EditorSceneManager.OpenScene(activeZone.Scene.Path);
						else
							EditorSceneManager.CloseScene(zone.Scene.Scene, true);
					}
				}
			}
		}
	}
}
