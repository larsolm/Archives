using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace PiRhoSoft.MonsterMaker
{
	public class ZoneLoader
	{
		public bool IsDone;
	}

	[AddComponentMenu("Monster Maker/World/World Manager")]
	public class WorldManager : Singleton<WorldManager>
	{
		[Tooltip("The World Asset that this defines the traits, zones, and layers in the world.")] public World World;

		[WorldState]
		[NonSerialized] public VariableStore PersistentState = new VariableStore();
		[NonSerialized] public VariableStore SessionState = new VariableStore();
		[NonSerialized] public string SaveFilename;

		public UnityAction<ZoneData> OnZoneEntered;
		public UnityAction<ZoneData> OnZoneLeft;

		public ZoneData CurrentZone { get; private set; }
		public ZoneData[] Zones { get; private set; }

		public bool IsPaused { get { return _pauseCount > 0; } }
		public bool IsLoading { get { return _pendingLoads > 0; } }

		private int _pauseCount = 0;
		private int _pendingLoads = 0;
		private List<ZoneData> _loadedZones = new List<ZoneData>();

		public ZoneData GetZone(UnityObject o)
		{
			var gameObject = o as GameObject;
			if (gameObject != null)
				return Zones[gameObject.scene.buildIndex];

			var behaviour = o as MonoBehaviour;
			if (behaviour != null)
				return Zones[behaviour.gameObject.scene.buildIndex];

			return null;
		}

		public void LoadWorld(GameManager.WorldData data)
		{
			PersistentState = data.WorldState ?? new VariableStore();

			foreach (var zone in World.Zones)
			{
				if (zone.Scene.Index < 0)
					Debug.LogFormat("zone {0} does not have a valid scene assigned", zone.name);
				else
					Zones[zone.Scene.Index].PersistentState = (zone.Scene.Index < data.ZoneStates.Length ? data.ZoneStates[zone.Scene.Index] : null) ?? new VariableStore();
			}
		}

		public void SaveWorld(GameManager.WorldData data)
		{
			data.WorldState = PersistentState;
			data.ZoneStates = new VariableStore[Zones.Length];

			for (var i = 0; i < Zones.Length; i++)
				data.ZoneStates[i] = Zones[i].PersistentState;
		}

		public void Pause()
		{
			Time.timeScale = 0.0f;
			_pauseCount++;
		}

		public void Unpause()
		{
			Time.timeScale = 1.0f;
			_pauseCount--;
		}

		public ZoneLoader ChangeZone(ZoneData to, bool waitForConnections)
		{
			var from = CurrentZone;
			var loader = new ZoneLoader { IsDone = false };

			if (from != null)
				LeaveZone(from, to);

			if (to != null)
			{
				if (to.Status == ZoneStatus.Loaded && !waitForConnections)
					EnterLoadedZone(loader, to, from);
				else
					StartCoroutine(EnterUnloadedZone(loader, from, to, waitForConnections));
			}
			else
			{
				loader.IsDone = true;
			}

			return loader;
		}

		public ZoneLoader TransitionZone(Zone zone, string spawnName, Transition transition, bool waitForConnections)
		{
			var to = Zones[zone.Scene.Index];
			var loader = new ZoneLoader();
			StartCoroutine(TransitionZone(loader, to, spawnName, transition, waitForConnections));
			return loader;
		}

		public void UpdateCurrentZone()
		{
			if (!IsInZone(Player.Instance.Mover, CurrentZone))
			{
				foreach (var zone in _loadedZones)
				{
					if (zone != CurrentZone && IsInZone(Player.Instance.Mover, zone))
					{
						ChangeZone(zone, false);
						return;
					}
				}
			}
		}

		protected override void OnAwake()
		{
			Zones = new ZoneData[SceneManager.sceneCountInBuildSettings];

			foreach (var zone in World.Zones)
				Zones[zone.Scene.Index] = new ZoneData(zone);
		}

		private IEnumerator LoadZone(ZoneData data)
		{
			while (data.Status == ZoneStatus.Unloading)
				yield return null;

			if (data.Status != ZoneStatus.Unloaded)
			{
				Debug.LogFormat("failed to load zone {0}: the zone is already loaded", data.Zone.name);
				yield break;
			}

			var loader = SceneManager.LoadSceneAsync(data.Zone.Scene.Index, LoadSceneMode.Additive);

			if (loader == null)
			{
				Debug.LogFormat("failed to load zone {0}: the zone has not been assigned a scene", data.Zone.name);
				yield break;
			}

			_pendingLoads++;
			data.Status = ZoneStatus.Loading;

			while (!loader.isDone)
				yield return null;

			if (data.Tilemap == null)
			{
				Debug.LogFormat("failed to load zone {0}: the zone does not contain MapProperties", data.Zone.name);
				SceneManager.UnloadSceneAsync(data.SceneIndex);
				data.Status = ZoneStatus.Unloaded;
			}
			else
			{
				data.Status = ZoneStatus.Loaded;
				_loadedZones.Add(data);
			}

			_pendingLoads--;
		}

		private IEnumerator UnloadZone(ZoneData data)
		{
			if (data.Status != ZoneStatus.Loaded)
			{
				Debug.LogFormat("attempting to unload zone {0} which is not loaded", data.Zone.name);
				yield break;
			}

			data.Status = ZoneStatus.Unloading;
			data.LoadedState.Reset();
			_loadedZones.Remove(data);

			var loader = SceneManager.UnloadSceneAsync(data.SceneIndex);

			if (loader == null)
			{
				Debug.LogFormat("failed to unload zone {0}", data.Zone.name);
				yield break;
			}

			while (!loader.isDone)
				yield return null;
			
			data.Status = ZoneStatus.Unloaded;
		}

		private void EnterLoadedZone(ZoneLoader loader, ZoneData to, ZoneData from)
		{
			StartCoroutine(LoadConnections(to, from));
			EnterZone(to, from);
			loader.IsDone = true;
		}

		private IEnumerator EnterUnloadedZone(ZoneLoader loader, ZoneData from, ZoneData to, bool waitForConnections)
		{
			Pause();

			if (to.Status == ZoneStatus.Unloaded || to.Status == ZoneStatus.Unloading)
				yield return StartCoroutine(LoadZone(to));

			while (to.Status == ZoneStatus.Loading)
				yield return null;

			if (waitForConnections)
				yield return StartCoroutine(LoadConnections(to, from));
			else
				StartCoroutine(LoadConnections(to, from));

			EnterZone(to, from);
			Unpause();
			loader.IsDone = true;
		}

		private void EnterZone(ZoneData to, ZoneData from)
		{
			if (to == CurrentZone)
			{
				Debug.LogFormat("attempting to enter zone {0} but it is already CurrentZone", from.Zone.name);
				return;
			}

			if (to.Zone.BackgroundMusic != null && (from == null || to.Zone.BackgroundMusic != from.Zone.BackgroundMusic))
				AudioManager.Instance.Push(to.Zone.BackgroundMusic, 1.0f, 1.0f, 1.0f);

			SceneManager.SetActiveScene(to.Zone.Scene.Scene);
			CurrentZone = to;

			if (OnZoneEntered != null)
				OnZoneEntered(to);
		}

		private void LeaveZone(ZoneData from, ZoneData to)
		{
			if (from != CurrentZone)
			{
				Debug.LogFormat("attempting to leave zone {0} but CurrentZone is {1}", from.Zone.name, CurrentZone.Zone.name);
				return;
			}

			if (OnZoneLeft != null)
				OnZoneLeft(from);

			CurrentZone = null;
			SceneManager.SetActiveScene(gameObject.scene);

			UnloadConnections(from, to);
			from.ActiveState.Reset();

			if (from.Zone.BackgroundMusic != null && (to == null || from.Zone.BackgroundMusic != to.Zone.BackgroundMusic))
				AudioManager.Instance.Pop(1.0f, 1.0f, 1.0f);
		}

		private IEnumerator LoadConnections(ZoneData to, ZoneData from)
		{
			foreach (var index in to.Connections)
			{
				var data = Zones[index];

				if (data.Status == ZoneStatus.Unloaded || data.Status == ZoneStatus.Unloading)
					StartCoroutine(LoadZone(data));
			}

			while (_pendingLoads > 0)
				yield return null;

			foreach (var loadedZone in _loadedZones)
			{
				var objects = loadedZone.Zone.Scene.Scene.GetRootGameObjects();

				foreach (var obj in objects)
					obj.SetActive(loadedZone.Zone.MapLayer == to.Zone.MapLayer);
			}
		}

		private void UnloadConnections(ZoneData from, ZoneData to)
		{
			foreach (var index in from.Connections)
			{
				var data = Zones[index];

				if (to == null || (data != to && !to.Connections.Contains(data.SceneIndex)))
					StartCoroutine(UnloadZone(data));
			}

			if (to == null || !to.Connections.Contains(from.SceneIndex))
				StartCoroutine(UnloadZone(from));
		}
		
		private IEnumerator TransitionZone(ZoneLoader loader, ZoneData to, string spawnName, Transition transition, bool waitForConnections)
		{
			Player.Instance.Controller.Freeze();

			yield return StartCoroutine(TransitionManager.Instance.StartTransition(transition));
			
			var zoneLoader = ChangeZone(to, false);

			yield return StartCoroutine(TransitionManager.Instance.ObscureTransition());

			while (!zoneLoader.IsDone)
				yield return null;

			var spawn = to.GetSpawnPoint(spawnName);
			Player.Instance.Mover.WarpToPosition(spawn.Position, spawn.Direction, spawn.Layer);

			yield return StartCoroutine(TransitionManager.Instance.FinishTransition());

			Player.Instance.Controller.Thaw();
			loader.IsDone = true;
		}

		private bool IsInZone(Mover mover, ZoneData zone)
		{
			return zone.Zone.MapLayer == CurrentZone.Zone.MapLayer
				&& zone.Tilemap.Bounds.Contains(mover.CurrentGridPosition);
		}
	}
}
