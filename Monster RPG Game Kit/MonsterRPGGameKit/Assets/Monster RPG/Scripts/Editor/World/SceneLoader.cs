using System;
using System.Collections.Generic;
using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PiRhoSoft.MonsterRpgEditor
{
	[Serializable]
	public class SceneLoaderState
	{
		public SceneState Scenes;
		public bool LoadWorld;
		public string MainScene;
		public string StartZone;
		public string StartSpawn;

		public void SetZone(Zone zone)
		{
			LoadWorld = true;
			MainScene = zone.World.MainScene.Path;
			StartZone = zone.name;
		}
	}

	[InitializeOnLoad]
	static class SceneLoader
	{
		public static readonly JsonPreference<SceneLoaderState> StatePreference = new JsonPreference<SceneLoaderState>("MonsterRpgGameKit.SceneLoader.State");
		public static readonly StringPreference FilePreference = new StringPreference("MonsterRpgGameKit.SceneLoader.File", "");
		public static readonly StringPreference ZonePreference = new StringPreference("MonsterRpgGameKit.SceneLoader.Zone", "");
		public static readonly StringPreference SpawnPreference = new StringPreference("MonsterRpgGameKit.SceneLoader.Spawn", "");
		public static readonly IntPreference ZoneTypePreference = new IntPreference("MonsterRpgGameKit.SceneLoader.ZoneType", 0);

		public const int LoadActiveZone = 0;
		public const int LoadSavedZone = 1;
		public const int LoadSpecificZone = 2;

		private const string _mainSceneNotSetError = "(EWSLMSNS) The World does not have a main scene set";
		private const string _noZonesError = "(EWSLNZ) The World does not have any zones";

		private static readonly GUIContent _failedToLoadContent = new GUIContent("Failed to load World");


		static SceneLoader()
		{
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}

		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			switch (state)
			{
				case PlayModeStateChange.ExitingEditMode: PlayModeStarting(); break;
				case PlayModeStateChange.EnteredPlayMode: PlayModeStarted(); break;
				case PlayModeStateChange.ExitingPlayMode: PlayModeEnding(); break;
				case PlayModeStateChange.EnteredEditMode: PlayModeEnded(); break;
			}
		}

		private static void PlayModeStarting()
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				var state = GetCurrentState();

				if (state.LoadWorld)
					EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

				StatePreference.Value = state;
			}
			else
			{
				EditorApplication.isPlaying = false;
			}
		}

		private static void PlayModeEnded()
		{
			SceneHelper.RestoreState(StatePreference.Value.Scenes);
		}

		private static void PlayModeStarted()
		{
			var state = StatePreference.Value;

			if (state.LoadWorld)
			{
				var hasScene = !string.IsNullOrEmpty(state.MainScene);
				var hasZone = !string.IsNullOrEmpty(state.StartZone);
				var hasSpawn = !string.IsNullOrEmpty(state.StartSpawn);

				if (!hasScene && hasZone)
				{
					PlayModeFailed(_mainSceneNotSetError);
				}
				else if (hasScene && !hasZone)
				{
					PlayModeFailed(_noZonesError);
				}
				else
				{
					var game = hasScene && hasZone ? new GameSaveData { MainScene = state.MainScene, StartingZone = state.StartZone, PlayerSpawn = hasSpawn ? new SpawnPoint { Name = state.StartSpawn } : SpawnPoint.Default } : null;
					var result = WorldLoader.Load(game, FilePreference.Value);

					result.OnError = PlayModeFailed;
				}
			}
		}

		private static void PlayModeFailed(string message)
		{
			EditorApplication.isPlaying = false;

			if (Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
				EditorWindow.GetWindow<SceneView>().ShowNotification(_failedToLoadContent);

			Debug.Log(message);
		}

		private static void PlayModeEnding()
		{
		}

		public static SceneLoaderState GetCurrentState()
		{
			var zonePath = ZonePreference.Value;
			var spawn = SpawnPreference.Value;
			var zoneType = ZoneTypePreference.Value;

			var state = new SceneLoaderState
			{
				Scenes = SceneHelper.CaptureState(),
				StartSpawn = spawn
			};

			if (zoneType == LoadActiveZone)
			{
				var zone = FindZone();
				if (zone == null)
				{
					var manager = FindWorldManager();

					if (manager != null && manager.World != null)
						zone = manager.World.Zones.Count > 0 ? manager.World.Zones[0] : null;
				}

				if (zone != null)
					state.SetZone(zone);
			}
			else if (zoneType == LoadSpecificZone)
			{
				var zones = AssetHelper.ListAssets<Zone>();
				var zone = GetZone(zones, zonePath);

				if (zone != null)
					state.SetZone(zone);
			}
			else
			{
				state.LoadWorld = zoneType == LoadSavedZone;
			}

			return state;
		}

		private static WorldManager FindWorldManager()
		{
			var manager = Object.FindObjectsOfType<WorldManager>();
			return manager != null && manager.Length > 0 ? manager[0] : null;
		}

		private static Zone FindZone()
		{
			var zones = AssetHelper.ListAssets<Zone>();
			var scene = SceneManager.GetActiveScene();
			var zone = GetZone(zones, scene.path);

			if (zone != null)
				return zone;

			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				scene = SceneManager.GetSceneAt(i);
				zone = GetZone(zones, scene.path);

				if (zone != null)
					return zone;
			}

			return null;
		}

		private static Zone GetZone(List<Zone> zones, string scenePath)
		{
			foreach (var zone in zones)
			{
				if (zone.Scene.Path == scenePath)
					return zone;
			}

			return null;
		}
	}
}
