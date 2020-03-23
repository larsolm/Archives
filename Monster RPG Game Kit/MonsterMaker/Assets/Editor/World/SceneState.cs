using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class SceneState
	{
		public static SceneState GetCurrentSceneState()
		{
			var manager = FindWorldManager();
			var state = new SceneState();

			state.MainScene = FindMainScene(manager);
			state.StartZone = FindStartZone(manager);
			state.Setup = EditorSceneManager.GetSceneManagerSetup();

			return state;
		}

		private static WorldManager FindWorldManager()
		{
			var manager = UnityObject.FindObjectsOfType<WorldManager>();
			return manager != null && manager.Length > 0 ? manager[0] : null;
		}

		private static string FindMainScene(WorldManager manager)
		{
			if (WorldPreferences.Instance.MainScene != null)
				return AssetDatabase.GetAssetPath(WorldPreferences.Instance.MainScene);

			if (manager != null)
				return manager.gameObject.scene.path;

			return "";
		}

		private static Zone GetZone(List<Zone> zones, Scene scene)
		{
			foreach (var zone in zones)
			{
				if (zone.Scene.Path == scene.path)
					return zone;
			}

			return null;
		}

		private static string FindStartZone(WorldManager manager)
		{
			if (WorldPreferences.Instance.StartingZone != null)
				return WorldPreferences.Instance.StartingZone.name;

			var zones = AssetFinder.ListSubAssets<World, Zone>();
			var scene = SceneManager.GetActiveScene();
			var zone = GetZone(zones, scene);

			if (zone != null)
				return zone.name;

			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				scene = SceneManager.GetSceneAt(i);
				zone = GetZone(zones, scene);

				if (zone != null)
					return zone.name;
			}

			if (manager != null && manager.World != null && manager.World.Zones.Count > 0)
				return manager.World.Zones[0].name;

			return null;
		}

		public string MainScene;
		public string StartZone;

		public SceneSetup[] Setup
		{
			get
			{
				var scenes = new SceneSetup[_scenes.Length];

				for (var i = 0; i < _scenes.Length; i++)
					scenes[i] = new SceneSetup { isActive = _scenes[i].IsActive, isLoaded = _scenes[i].IsLoaded, path = _scenes[i].Path };

				return scenes;
			}
			set
			{
				_scenes = new SceneData[value.Length];

				for (var i = 0; i < value.Length; i++)
					_scenes[i] = new SceneData { IsActive = value[i].isActive, IsLoaded = value[i].isLoaded, Path = value[i].path };
			}
		}

		public void Restore()
		{
			EditorSceneManager.RestoreSceneManagerSetup(Setup);
		}

		[Serializable]
		private class SceneData
		{
			public bool IsActive;
			public bool IsLoaded;
			public string Path;
		}

		[SerializeField] private SceneData[] _scenes;
	}
}
