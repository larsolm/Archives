using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public abstract class PreferenceSet
	{
		public string Name { get; private set; }
		public string PreferenceName { get; private set; }

		public void Load()
		{
			var json = EditorPrefs.GetString(PreferenceName);
			EditorJsonUtility.FromJsonOverwrite(json, this);
		}

		public void Save()
		{
			var json = EditorJsonUtility.ToJson(this);
			EditorPrefs.SetString(PreferenceName, json);
		}

		public abstract void OnGUI();

		protected PreferenceSet()
		{
			Name = GetType().Name;
			PreferenceName = "MonsterMaker." + Name;
			Load();
		}
	}

	public class PreferencesAttribute : Attribute
	{
	}

	[Preferences]
	public class WorldPreferences : PreferenceSet
	{
		public static WorldPreferences Instance = new WorldPreferences();

		private const string _mainSceneTooltip = "The scene containing your WorldManager and Player to load when entering Play mode. Leave this empty to load the scenes that are open in the editor.";
		private const string _startingZoneTooltip = "The zone to start in when entering Play mode. Leave this empty to start in the scene open in the editor.";
		private const string _startingSpawnTooltip = "The spawn point to place the player at when entering Play mode. Leave this empty to start at a random spawn point.";

		public SceneAsset MainScene;
		public Zone StartingZone;
		public string StartingSpawn;

		public override void OnGUI()
		{
			MainScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Main Scene", _mainSceneTooltip), MainScene, typeof(SceneAsset), false);

			if (MainScene != null)
			{
				StartingZone = (Zone)EditorGUILayout.ObjectField(new GUIContent("Starting Zone", _startingZoneTooltip), StartingZone, typeof(Zone), false);
				StartingSpawn = EditorGUILayout.TextField(new GUIContent("Starting Spawn", _startingSpawnTooltip), StartingSpawn);
			}
		}
	}

	[Preferences]
	public class BattlePreferences : PreferenceSet
	{
		public static BattlePreferences Instance = new BattlePreferences();

		public SceneAsset BattleScene;

		public override void OnGUI()
		{
			BattleScene = (SceneAsset)EditorGUILayout.ObjectField("Battle Scene", BattleScene, typeof(SceneAsset), false);
		}
	}

	[Preferences]
	public class UiPreferences : PreferenceSet
	{
		public static UiPreferences Instance = new UiPreferences();

		public List<string> ListenerCategories = new List<string>();

		public override void OnGUI()
		{
			ListenerCategoryWindow.Draw();
		}
	}

	public class PreferencesWindow : EditorWindow
	{
		[MenuItem("Edit/Project Settings/Monster Maker")]
		[MenuItem("Window/Monster Maker/Settings")]
		public static void ShowWindow()
		{
			var window = GetWindow<PreferencesWindow>("Monster Maker");
			window.Show();
		}

		private void OnEnable()
		{
			var preferenceTypes = TypeFinder.GetTypesWithAttribute<PreferenceSet, PreferencesAttribute>();

			foreach (var preferenceType in preferenceTypes)
			{
				var instanceField = preferenceType.GetField("Instance");
				var set = instanceField.GetValue(null) as PreferenceSet;
				_sets.Add(set);
			}
		}

		private void OnGUI()
		{
			foreach (var set in _sets)
			{
				using (var check = new EditorGUI.ChangeCheckScope())
				{
					EditorGUILayout.LabelField(set.Name);

					using (new EditorGUI.IndentLevelScope())
						set.OnGUI();

					if (check.changed)
						set.Save();
				}

				EditorGUILayout.Space();
			}
		}

		private List<PreferenceSet> _sets = new List<PreferenceSet>();
	}
}
