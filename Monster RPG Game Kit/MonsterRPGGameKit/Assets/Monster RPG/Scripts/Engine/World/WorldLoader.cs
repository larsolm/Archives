using System;
using System.Collections;
using System.IO;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.MonsterRpgEngine
{
	public enum LoadState
	{
		ReadingData,
		LoadingWorld,
		LoadingZones,
		LoadingUi,
		Error,
		Complete
	}

	public enum SaveState
	{
		SavingWorld,
		WritingData,
		Error,
		Complete
	}

	public class LoadInformation
	{
		public Action OnComplete;
		public Action<LoadState, float> OnProgress;
		public Action<string> OnError;

		public LoadState State { get; private set; }
		public float Progress { get; private set; }
		public string Message { get; private set; }

		public void UpdateProgress(LoadState state, float progress)
		{
			State = state;
			Progress = progress;

			OnProgress?.Invoke(state, progress);
		}

		public void SetError(string message)
		{
			State = LoadState.Error;
			Progress = 0.0f;
			Message = message;

			OnError?.Invoke(message);
		}

		public void SetComplete()
		{
			State = LoadState.Complete;
			Progress = 1.0f;

			OnComplete?.Invoke();
		}
	}

	public class SaveInformation
	{
		public Action OnComplete;
		public Action<SaveState, float> OnProgress;
		public Action<string> OnError;

		public SaveState State { get; private set; }
		public float Progress { get; private set; }
		public string Message { get; private set; }

		public void UpdateProgress(SaveState state, float progress)
		{
			State = state;
			Progress = progress;

			OnProgress?.Invoke(state, progress);
		}

		public void SetError(string message)
		{
			State = SaveState.Error;
			Progress = 0.0f;
			Message = message;

			OnError?.Invoke(message);
		}

		public void SetComplete()
		{
			State = SaveState.Complete;
			Progress = 1.0f;

			OnComplete?.Invoke();
		}
	}

	[Serializable]
	public class SaveData
	{
		public GameSaveData Game = new GameSaveData();
		public WorldSaveData World = new WorldSaveData();
		public PlayerSaveData Player = new PlayerSaveData();
	}

	[Serializable]
	public class GameSaveData
	{
		public string MainScene;
		public string StartingZone;
		public SpawnPoint PlayerSpawn;
	}

	[HelpURL(MonsterRpg.DocumentationUrl + "world-loader")]
	public class WorldLoader : GlobalBehaviour<WorldLoader>
	{
		private const string _invalidLoadError = "(WWLIL) Failed to load save file {0}: {1}";
		private const string _invalidSaveError = "(WWLIS) Failed to write save file {0}: {1}";
		private const string _invalidZoneError = "(WWLIZ) Failed to load starting zone: the zone {0} does not exist on the world in scene {1}";
		private const string _missingCameraError = "(WWLMC) Failed to load world: the scene {0} does not contain a Camera";
		private const string _missingCompositionManagerError = "(WWLMCM) Failed to load world: the scene {0} does not contain a CompositionManager";
		private const string _missingMainSceneError = "(WWLMMS) Failed to load world: the main scene {0} could not be found";
		private const string _missingPlayerError = "(WWLMP) Failed to load world: the scene {0} does not contain a Player";
		private const string _missingWorldAssetError = "(WWLMWA) Failed to load world: the WorldManager in scene {0} does not have a World set";
		private const string _missingWorldManagerError = "(WWLMWM) Failed to load world: the scene {0} does not contain a WorldManager";

		public static LoadInformation New(string startingZone, string playerSpawn)
		{
			var filename = Guid.NewGuid().ToString();
			var game = new GameSaveData
			{
				StartingZone = startingZone,
				PlayerSpawn = new SpawnPoint { Name = playerSpawn }
			};

			return Load(game, filename);
		}

		public static LoadInformation Load(string filename)
		{
			return Load(null, filename);
		}

		public static LoadInformation Load(GameSaveData game, string filename)
		{
			var info = new LoadInformation();

			Instance.StartCoroutine(Instance.Load(info, game, filename));

			return info;
		}

		public static SaveInformation Save(WorldManager world)
		{
			var info = new SaveInformation();
			Instance.Save(info, world);
			return info;
		}

		private IEnumerator Load(LoadInformation info, GameSaveData game, string filename)
		{
			info.UpdateProgress(LoadState.ReadingData, 0.0f);

			var data = new SaveData();

			if (!string.IsNullOrEmpty(filename))
				Read(info, filename, data);

			if (info.State == LoadState.Error)
				yield break;

			game = game ?? data.Game;
			var worldData = data.World ?? new WorldSaveData();
			var playerData = data.Player ?? new PlayerSaveData();

			info.UpdateProgress(LoadState.LoadingWorld, 0.1f);

			var mainLoader = string.IsNullOrEmpty(game.MainScene) ? null : SceneManager.LoadSceneAsync(game.MainScene);

			if (mainLoader == null)
			{
				info.SetError(string.Format(_missingMainSceneError, game.MainScene));
				yield break;
			}

			while (!mainLoader.isDone)
				yield return null;

			if (WorldManager.Instance == null)
			{
				info.SetError(string.Format(_missingWorldManagerError, game.MainScene));
				yield break;
			}

			if (WorldManager.Instance.World == null)
			{
				info.SetError(string.Format(_missingWorldAssetError, game.MainScene));
				yield break;
			}

			if (InstructionManager.Instance == null)
			{
				info.SetError(string.Format(_missingCompositionManagerError, game.MainScene));
				yield break;
			}

			if (Player.Instance == null)
			{
				info.SetError(string.Format(_missingPlayerError, game.MainScene));
				yield break;
			}

			var camera = ComponentHelper.GetComponentInScene<Camera>(WorldManager.Instance.gameObject.scene.buildIndex, false);
			if (camera == null)
			{
				info.SetError(string.Format(_missingCameraError, game.MainScene));
				yield break;
			}

			var zone = WorldManager.Instance.World.GetZoneByName(game.StartingZone);

			if (zone == null)
			{
				info.SetError(string.Format(_invalidZoneError, game.StartingZone, game.MainScene));
				yield break;
			}

			info.UpdateProgress(LoadState.LoadingUi, 0.2f);

			yield return WorldManager.Instance.LoadUi();

			info.UpdateProgress(LoadState.LoadingZones, 0.4f);

			WorldManager.Instance.Load(filename, worldData);
			Player.Instance.Load(playerData);

			WorldManager.Instance.TransitionZone(zone, game.PlayerSpawn, null);

			while (WorldManager.Instance.IsTransitioning)
				yield return null;

			info.SetComplete();
		}

		private void Save(SaveInformation info, WorldManager world)
		{
			info.UpdateProgress(SaveState.SavingWorld, 0.0f);

			var direction = Player.Instance.Mover.MovementDirection;
			var position = Player.Instance.Mover.CurrentGridPosition;
			var layer = Player.Instance.Mover.MovementLayer;

			var data = new SaveData();
			data.Game.MainScene = world.gameObject.scene.path;
			data.Game.StartingZone = Player.Instance.Zone.Zone.name;
			data.Game.PlayerSpawn.Direction = direction;
			data.Game.PlayerSpawn.Position = position;
			data.Game.PlayerSpawn.Layer = layer;

			Player.Instance.Save(data.Player);

			var filename = world.Save(data.World);

			info.UpdateProgress(SaveState.WritingData, 0.2f);
			Write(info, data, filename);

			if (info.State != SaveState.Error)
				info.SetComplete();
		}

		// These could be threaded but the Unity web platform doesn't support System.Thread and all platforms warn when
		// reading and writing Json from a background thread despite seeming to work correctly.

		private static void Read(LoadInformation info, string filename, SaveData data)
		{
			try
			{
				var json = File.ReadAllText(filename);
				JsonUtility.FromJsonOverwrite(json, data);
			}
			catch (Exception e)
			{
				info.SetError(string.Format(_invalidLoadError, filename, e.Message));
			}
		}

		private static void Write(SaveInformation info, SaveData data, string filename)
		{
			try
			{
				var json = JsonUtility.ToJson(data, true);
				File.WriteAllText(filename, json);
			}
			catch (Exception e)
			{
				info.SetError(string.Format(_invalidSaveError, filename, e.Message));
			}
		}
	}
}
