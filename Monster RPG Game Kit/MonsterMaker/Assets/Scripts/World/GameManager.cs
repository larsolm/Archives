using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.MonsterMaker
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

			if (OnProgress != null)
				OnProgress(state, progress);
		}

		public void SetError(string message)
		{
			State = LoadState.Error;
			Progress = 0.0f;
			Message = message;

			if (OnError != null)
				OnError(message);
		}

		public void SetComplete()
		{
			State = LoadState.Complete;
			Progress = 1.0f;

			if (OnComplete != null)
				OnComplete();
		}
	}

	public enum SaveState
	{
		SavingWorld,
		WritingData,
		Error,
		Complete
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

			if (OnProgress != null)
				OnProgress(state, progress);
		}

		public void SetError(string message)
		{
			State = SaveState.Error;
			Progress = 0.0f;
			Message = message;

			if (OnError != null)
				OnError(message);
		}

		public void SetComplete()
		{
			State = SaveState.Complete;
			Progress = 1.0f;

			if (OnComplete != null)
				OnComplete();
		}
	}

	public class GameManager : MonoBehaviour
	{
		[Serializable]
		public class SaveData
		{
			public GameData Game;
			public WorldData World;
			public PlayerData Player;
		}

		[Serializable]
		public class GameData
		{
			public string MainScene;
			public string StartingZone;
			public SpawnPoint PlayerSpawn;
		}

		[Serializable]
		public class WorldData
		{
			public VariableStore WorldState = new VariableStore();
			public VariableStore[] ZoneStates = new VariableStore[0];
		}

		[Serializable]
		public class PlayerData
		{
		}

		public static LoadInformation New(string mainScene, string startingZone, string playerSpawn)
		{
			var filename = Guid.NewGuid().ToString();
			var game = new GameData();
			var world = new WorldData();

			game.MainScene = mainScene;
			game.StartingZone = startingZone;
			game.PlayerSpawn.Name = playerSpawn;

			return Load(game, world, null, filename);
		}

		public static LoadInformation Load(string filename)
		{
			return Load(null, null, null, filename);
		}

		public static LoadInformation Load(GameData game)
		{
			return Load(game, null, null, null);
		}

		public static SaveInformation Save(WorldManager world)
		{
			return Save(world, world.SaveFilename);
		}

		private static GameManager _instance;

		private static GameManager Instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject gameObject = new GameObject();
					_instance = gameObject.AddComponent<GameManager>();
					gameObject.hideFlags = HideFlags.HideInHierarchy;
					DontDestroyOnLoad(gameObject);
				}

				return _instance;
			}
		}

		private static LoadInformation Load(GameData game, WorldData world, PlayerData player, string filename)
		{
			var info = new LoadInformation();
			Instance.StartCoroutine(Instance.Load(info, game, world, player, filename));
			return info;
		}

		private static SaveInformation Save(WorldManager world, string filename)
		{
			var info = new SaveInformation();
			Instance.StartCoroutine(Instance.Save(info, world, filename));
			return info;
		}

		private IEnumerator Load(LoadInformation info, GameData game, WorldData world, PlayerData player, string filename)
		{
			if (world == null)
			{
				info.UpdateProgress(LoadState.ReadingData, 0.0f);

				var data = new SaveData();

				if (!string.IsNullOrEmpty(filename))
					yield return StartCoroutine(Read(info, filename, data));

				if (info.State == LoadState.Error)
					yield break;

				game = game ?? data.Game;
				world = data.World ?? new WorldData();
				player = data.Player ?? new PlayerData();
			}

			info.UpdateProgress(LoadState.LoadingWorld, 0.1f);

			var mainLoader = SceneManager.LoadSceneAsync(game.MainScene);

			if (mainLoader == null)
			{
				info.SetError(string.Format("the scene {0} could not be loaded", game.MainScene));
				yield break;
			}

			while (!mainLoader.isDone)
				yield return null;

			if (WorldManager.Instance == null)
			{
				SceneManager.UnloadSceneAsync(game.MainScene);
				info.SetError(string.Format("the scene {0} does not contain a WorldManager", game.MainScene));
				yield break;
			}

			if (WorldManager.Instance.World == null)
			{
				SceneManager.UnloadSceneAsync(game.MainScene);
				info.SetError(string.Format("the WorldManager in scene {0} does not have a World set", game.MainScene));
				yield break;
			}

			if (Player.Instance == null)
			{
				SceneManager.UnloadSceneAsync(game.MainScene);
				info.SetError(string.Format("the scene {0} does not contain a Player", game.MainScene));
				yield break;
			}

			var zone = WorldManager.Instance.World.GetZone(game.StartingZone);

			if (zone == null)
			{
				SceneManager.UnloadSceneAsync(game.MainScene);
				info.SetError(string.Format("the zone {0} does not exist on the world in scene {1}", game.StartingZone, game.MainScene));
				yield break;
			}

			WorldManager.Instance.SaveFilename = filename;
			WorldManager.Instance.LoadWorld(world);
			Player.Instance.Initialize(player);

			var zoneData = WorldManager.Instance.Zones[zone.Scene.Index];
			info.UpdateProgress(LoadState.LoadingZones, 0.2f);
			var zoneLoader = WorldManager.Instance.ChangeZone(zoneData, true);

			while (!zoneLoader.IsDone)
				yield return null;

			var spawn = game.PlayerSpawn.Layer == CollisionLayer.None ? zoneData.GetSpawnPoint(game.PlayerSpawn.Name ?? "") : game.PlayerSpawn;
			Player.Instance.Mover.WarpToPosition(spawn.Position, spawn.Direction, spawn.Layer);

			info.UpdateProgress(LoadState.LoadingUi, 0.9f);

			yield return StartCoroutine(LoadUis(WorldManager.Instance.World.UIScenes));

			info.SetComplete();
		}

		public IEnumerator Save(SaveInformation info, WorldManager world, string filename)
		{
			info.UpdateProgress(SaveState.SavingWorld, 0.0f);

			var direction = Player.Instance.Mover.MoveDirection;
			var position = Player.Instance.Mover.CurrentGridPosition;
			var layer = Player.Instance.Mover.CollisionLayer;

			var data = new SaveData();
			data.Game.MainScene = world.gameObject.scene.path;
			data.Game.StartingZone = world.CurrentZone.Zone.name;
			data.Game.PlayerSpawn.Direction = direction;
			data.Game.PlayerSpawn.Position = position;
			data.Game.PlayerSpawn.Layer = layer;

			world.SaveWorld(data.World);

			info.UpdateProgress(SaveState.WritingData, 0.2f);
			yield return StartCoroutine(Write(info, data, filename));

			if (info.State != SaveState.Error)
				info.SetComplete();
		}
		
		private int _pendingUis = 0;

		private IEnumerator LoadUis(List<SceneReference> scenes)
		{
			foreach(var scene in scenes)
				StartCoroutine(LoadUI(scene.Path));

			while (_pendingUis > 0)
				yield return null;
		}

		private IEnumerator LoadUI(string path)
		{
			var loader = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);

			if (loader == null)
			{
				Debug.LogFormat("failed to load ui {0}: the zone has not been assigned a scene", path);
				yield break;
			}
			
			_pendingUis++;

			while (!loader.isDone)
				yield return null;

			_pendingUis--;
		}

		private class ThreadData
		{
			public bool Failed;
			public string Error;
			public SaveData Contents;
			public string Filename;
			public string Path;
		}

		private static IEnumerator Read(LoadInformation info, string filename, SaveData data)
		{
			var thread = new Thread(DoRead);
			var transfer = new ThreadData { Failed = false, Contents = data, Path = filename, Filename = filename };
			thread.Start(transfer);

			while (thread.IsAlive)
				yield return null;

			if (transfer.Failed)
				info.SetError(transfer.Error);
		}

		private static IEnumerator Write(SaveInformation info, SaveData data, string filename)
		{
			var thread = new Thread(DoWrite);
			var transfer = new ThreadData { Failed = false, Contents = data, Path = filename, Filename = filename };
			thread.Start(transfer);

			while (thread.IsAlive)
				yield return null;

			if (transfer.Failed)
				info.SetError(transfer.Error);
		}

		private static void DoRead(object transfer)
		{
			var data = transfer as ThreadData;

			try
			{
				var json = File.ReadAllText(data.Path);
				data.Contents = JsonUtility.FromJson<SaveData>(json);
			}
			catch (Exception e)
			{
				data.Failed = true;
				data.Error = string.Format("unable to read save file {0}: {1}", data.Filename, e.Message);
			}
		}

		private static void DoWrite(object transfer)
		{
			var data = transfer as ThreadData;

			try
			{
				var json = JsonUtility.ToJson(data.Contents);
				File.WriteAllText(data.Path, json);
			}
			catch (Exception e)
			{
				data.Failed = true;
				data.Error = string.Format("unable to write save file {0}: {1}", data.Filename, e.Message);
			}
		}
	}
}
