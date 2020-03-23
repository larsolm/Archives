using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[InitializeOnLoad]
	static class SceneLoader
	{
		static SceneLoader()
		{
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}

		private const string _saveFileToLoadPreference = "MonsterMaker.SaveFileToLoad";
		private const string _editorScenesPreference = "MonsterMaker.SceneLoader.EditorScenes";

		public static string SaveFileToLoad
		{
			get { return EditorPrefs.GetString(_saveFileToLoadPreference); }
			set { EditorPrefs.SetString(_saveFileToLoadPreference, value); }
		}

		private static SceneState _state
		{
			get
			{
				var json = EditorPrefs.GetString(_editorScenesPreference);
				var state = JsonUtility.FromJson<SceneState>(json);
				return state;
			}
			set
			{
				var json = JsonUtility.ToJson(value);
				EditorPrefs.SetString(_editorScenesPreference, json);
			}
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
			var state = SceneState.GetCurrentSceneState();

			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				if (!string.IsNullOrEmpty(SaveFileToLoad) || !string.IsNullOrEmpty(state.MainScene))
					EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

				_state = state;
			}
			else
			{
				EditorApplication.isPlaying = false;
			}
		}

		private static void PlayModeEnded()
		{
			_state.Restore();
		}

		private static void PlayModeStarted()
		{
			var saveFile = SaveFileToLoad;
			var state = _state;

			if (!string.IsNullOrEmpty(saveFile))
			{
				var result = GameManager.Load(saveFile);
				result.OnError = PlayModeFailed;
			}
			else if (!string.IsNullOrEmpty(state.MainScene))
			{
				if (state.StartZone != null)
				{
					var spawn = new SpawnPoint { Name = WorldPreferences.Instance.StartingSpawn };
					var game = new GameManager.GameData { MainScene = state.MainScene, StartingZone = state.StartZone, PlayerSpawn = spawn };
					var result = GameManager.Load(game);

					result.OnError = PlayModeFailed;
				}
				else
				{
					PlayModeFailed("the World does not have any zones");
				}
			}
		}

		private static void PlayModeFailed(string message)
		{
			EditorApplication.isPlaying = false;

			if (Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
				EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent("Failed to load World"));

			Debug.Log(message);
		}

		private static void PlayModeEnding()
		{
		}
	}
}
