using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class WorldSaveData
	{
		public VariableList PersistentVariables = new VariableList();
		public List<ZoneSaveData> Zones = new List<ZoneSaveData>();
	}

	public class ZoneLoadStatus
	{
		public bool IsDone;
	}

	[DisallowMultipleComponent]
	[HelpURL(MonsterRpg.DocumentationUrl + "world-manager")]
	[AddComponentMenu("PiRho Soft/Managers/World Manager")]
	public class WorldManager : SingletonBehaviour<WorldManager>, IVariableStore, IVariableListener
	{
		private const string _zoneAlreadyLoadedError = "(WWMZAL) Failed to load zone {0}: the zone is already loaded";
		private const string _zoneNotAssignedError = "(WWMZNA) Failed to load zone {0}: the zone has not been assigned a scene";
		private const string _zoneNotLoadedError = "(WWMZNL) Failed to unload zone {0}: the zone is not loaded";
		private const string _zoneUnloadFailedError = "(WWMZUF) Failed to unload zone {0}";
		private const string _missingZoneSceneWarning = "(WWMMZS) Zone {0} does not have a valid scene assigned";

		[Tooltip("The world asset that this manager will load zones and scenes from")]
		[AssetPopup]
		public World World;

		public Interface Interface { get; private set; }
		public ZoneData[] Zones { get; private set; }
		public List<ZoneData> LoadedZones { get; private set; }
		public string SaveFilename { get; private set; }

		private Dictionary<Vector2Int, CollisionLayer> _occupiedTiles = new Dictionary<Vector2Int, CollisionLayer>();
		private Dictionary<Vector2Int, Interaction> _interactionTiles = new Dictionary<Vector2Int, Interaction>();

		private VariableList _variables = new VariableList();
		private MappedVariableStore _variableStore = new MappedVariableStore();
		private InstructionContext _context = new InstructionContext();

		private int _freezeCount = 0;
		private int _transitionCount = 0;
		private int _pendingLoads = 0;

		protected override void Awake()
		{
			base.Awake();

			CreateZones();
			SetupVariables();
		}

		protected override void OnDestroy()
		{
			TeardownVariables();
			DestroyZones();

			base.OnDestroy();
		}

		#region Zones

		private void CreateZones()
		{
			Zones = new ZoneData[SceneManager.sceneCountInBuildSettings];
			LoadedZones = new List<ZoneData>();

			foreach (var zone in World.Zones)
			{
				var index = zone.Scene.Index;

				if (index < 0)
					Debug.LogWarningFormat(zone, _missingZoneSceneWarning, zone.name);
				else
					Zones[index] = CreateZone(zone, index);
			}
		}

		private void DestroyZones()
		{
			foreach (var zone in Zones)
			{
				if (zone != null)
					DestroyZone(zone);
			}
		}

		private ZoneData CreateZone(Zone zone, int index)
		{
			var data = ScriptableObject.CreateInstance<ZoneData>();
			data.Setup(this, zone, index);
			return data;
		}

		private void DestroyZone(ZoneData zone)
		{
			Destroy(zone);
		}

		public ZoneData GetZone(Zone zone)
		{
			var index = zone.Scene.Index;
			return index >= 0 && index < Zones.Length ? Zones[index] : null;
		}

		public ZoneData GetZone(Object o)
		{
			switch (o)
			{
				case GameObject gameObject: return Zones[gameObject.scene.buildIndex];
				case MonoBehaviour behaviour: return Zones[behaviour.gameObject.scene.buildIndex];
				case ZoneData zone: return zone;
				default: return null;
			}
		}

		#endregion

		#region Variables

		private static VariableMap _variableMap;
		private static PropertyMap<WorldManager> _propertyMap;

		public MappedVariableStore Variables => _variableStore;
		public InstructionContext Context => _context;

		protected static VariableValue GetPlayer(WorldManager world) => VariableValue.Create(Player.Instance);
		protected static VariableValue GetZone(WorldManager world) => VariableValue.Create(Player.Instance.Zone);
		protected static VariableValue GetZone(WorldManager world, int index) => VariableValue.Create(world.Zones[index]);

		protected void AddPropertiesToMap<WorldManagerType>(PropertyMap<WorldManagerType> map) where WorldManagerType : WorldManager
		{
			map.Add(nameof(Player), GetPlayer, null);
			map.Add(nameof(Player.Instance.Zone), GetZone, null);

			for (var i = 0; i < Zones.Length; i++)
			{
				var index = i; // For capturing
				var zone = Zones[i];

				if (zone != null)
					map.Add(zone.Zone.name, world => GetZone(world, index), null);
			}
		}

		protected virtual void SetupVariables()
		{
			if (_propertyMap == null)
			{
				_propertyMap = new PropertyMap<WorldManager>();
				AddPropertiesToMap(_propertyMap);
			}

			if (_variableMap == null || _variableMap.Version != World.WorldSchema.Version)
			{
				_variableMap = new VariableMap(World.WorldSchema.Version)
					.Add(_propertyMap)
					.Add(World.WorldSchema);
			}

			_variableStore.Setup(_variableMap, new PropertyList<WorldManager>(this, _propertyMap), new VariableListener(this, _variables));
			_variables.Setup(World.WorldSchema, this);
		}

		protected virtual void TeardownVariables()
		{
			_context.Clear();
		}

		#endregion

		#region Persistence

		public virtual void Load(string filename, WorldSaveData saveData)
		{
			_context.SetStore(nameof(World), this);
			_context.SetStore(nameof(Player), Player.Instance);

			if (AudioManager.Instance && World.BackgroundMusic)
				AudioManager.Instance.Push(World.BackgroundMusic, 0.0f, 1.0f, 0.0f);

			_variables.LoadFrom(saveData.PersistentVariables, VariableDefinition.Saved);

			foreach (var zoneData in saveData.Zones)
			{
				var zone = World.GetZoneByName(zoneData.Name);
				var data = zone != null ? GetZone(zone) : null;

				if (data != null)
					data.Load(zoneData);
			}

			SaveFilename = filename;
		}

		public virtual string Save(WorldSaveData saveData)
		{
			_variables.SaveTo(saveData.PersistentVariables, VariableDefinition.Saved);

			foreach (var zone in Zones)
			{
				if (zone != null)
				{
					var zoneSaveData = new ZoneSaveData();
					zone.Save(zoneSaveData);
					zoneSaveData.Name = zone.Zone.name;
					saveData.Zones.Add(zoneSaveData);
				}
			}

			return SaveFilename;
		}

		#endregion

		#region Freezing

		public bool IsFrozen => _freezeCount > 0;

		public void Freeze()
		{
			Time.timeScale = 0.0f;
			_freezeCount++;
		}

		public void Thaw()
		{
			_freezeCount--;

			if (_freezeCount == 0)
				Time.timeScale = 1.0f;
		}

		#endregion

		#region Tiles

		public TileInfo FindTile(Vector2Int position)
		{
			if (Player.Instance != null && Player.Instance.Zone != null)
			{
				foreach (var zone in LoadedZones)
				{
					if (zone.Zone.MapLayer == Player.Instance.Zone.Zone.MapLayer)
					{
						var tile = zone.Properties.GetTile(position);
						if (tile != null)
							return tile;
					}
				}
			}

			return null;
		}

		public bool IsOccupied(Vector2Int position, CollisionLayer layer)
		{
			if (_occupiedTiles.TryGetValue(position, out CollisionLayer occupied))
				return (occupied & layer) > 0;

			return false;
		}

		public void SetOccupied(Vector2Int position, CollisionLayer layer)
		{
			if (_occupiedTiles.TryGetValue(position, out CollisionLayer occupied))
				occupied |= layer;
			else
				_occupiedTiles[position] = layer;
		}

		public void SetUnoccupied(Vector2Int position, CollisionLayer layer)
		{
			if (_occupiedTiles.TryGetValue(position, out CollisionLayer occupied))
			{
				var tile = FindTile(position);
				occupied &= ~layer;

				if (occupied == CollisionLayer.None)
					_occupiedTiles.Remove(position);
			}
		}

		public IInteractable GetInteraction(Vector2Int position)
		{
			if (_interactionTiles.TryGetValue(position, out Interaction interaction))
				return interaction;

			var tile = FindTile(position);
			return tile?.Instructions;
		}

		public void AddInteraction(Vector2Int position, Interaction interaction)
		{
			if (interaction)
				_interactionTiles[position] = interaction;
		}

		public void RemoveInteraction(Vector2Int position, Interaction interaction)
		{
			if (interaction)
				_interactionTiles.Remove(position);
		}

		#endregion

		#region Zone Loading

		public IEnumerator LoadUi()
		{
			foreach (var scene in World.UiScenes)
			{
				if (scene.IsAssigned)
				{
					var loader = SceneManager.LoadSceneAsync(scene.Path, LoadSceneMode.Additive);
					if (loader != null)
					{
						while (!loader.isDone)
							yield return null;
					}
				}
			}
		}

		private IEnumerator LoadZone(ZoneData data)
		{
			while (data.State == ZoneState.Unloading)
				yield return null;

			if (data.State != ZoneState.Unloaded)
			{
				Debug.LogErrorFormat(this, _zoneAlreadyLoadedError, data.Zone.name);
				yield break;
			}

			var loader = SceneManager.LoadSceneAsync(data.Zone.Scene.Index, LoadSceneMode.Additive);

			if (loader == null)
			{
				Debug.LogErrorFormat(this, _zoneNotAssignedError, data.Zone.name);
				yield break;
			}

			_pendingLoads++;
			data.State = ZoneState.Loading;

			while (!loader.isDone)
				yield return null;

			var valid = data.Loaded();

			if (valid)
			{
				data.State = ZoneState.Loaded;
				LoadedZones.Add(data);
			}
			else
			{
				SceneManager.UnloadSceneAsync(data.SceneIndex);
				data.State = ZoneState.Unloaded;
			}

			_pendingLoads--;
		}

		private IEnumerator UnloadZone(ZoneData data)
		{
			if (data.State != ZoneState.Loaded)
			{
				Debug.LogErrorFormat(this, _zoneNotLoadedError, data.Zone.name);
				yield break;
			}

			data.Unloading();
			data.State = ZoneState.Unloading;
			LoadedZones.Remove(data);

			var loader = SceneManager.UnloadSceneAsync(data.SceneIndex);

			if (loader == null)
			{
				Debug.LogErrorFormat(this, _zoneUnloadFailedError, data.Zone.name);
				yield break;
			}

			while (!loader.isDone)
				yield return null;

			data.State = ZoneState.Unloaded;
		}

		private void EnteringLoadedZone(ZoneLoadStatus status, ZoneData to, ZoneData from)
		{
			StartCoroutine(LoadConnections(to, from));
			EnteringZone(to, from);
			status.IsDone = true;
		}

		private IEnumerator EnteringUnloadedZone(ZoneLoadStatus status, ZoneData from, ZoneData to, bool waitForConnections)
		{
			Freeze();

			if (to.State == ZoneState.Unloaded || to.State == ZoneState.Unloading)
				yield return LoadZone(to);

			while (to.State == ZoneState.Loading)
				yield return null;

			if (waitForConnections)
				yield return LoadConnections(to, from);
			else
				StartCoroutine(LoadConnections(to, from));

			EnteringZone(to, from);
			status.IsDone = true;

			Thaw();
		}

		private IEnumerator LoadConnections(ZoneData to, ZoneData from)
		{
			foreach (var index in to.Connections)
			{
				var data = Zones[index];

				if (data.State == ZoneState.Unloaded || data.State == ZoneState.Unloading)
					StartCoroutine(LoadZone(data));
			}

			while (_pendingLoads > 0)
				yield return null;

			foreach (var loadedZone in LoadedZones)
			{
				var enabled = loadedZone.Zone.MapLayer == to.Zone.MapLayer;

				if (enabled && !loadedZone.IsEnabled)
				{
					loadedZone.Enabled();
					yield return null; // wait for Start and OnEnabled calls
					loadedZone.RestoreNpcData();
				}

				if (!enabled && loadedZone.IsEnabled)
				{
					loadedZone.PersistNpcData();
					loadedZone.Disabled();
				}
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

		#endregion

		#region Zone Changing

		public bool IsTransitioning => _transitionCount > 0;

		public void ChangeZone(Zone zone)
		{
			LeavingZone();
	
			var to = GetZone(zone);
			var status = ChangeZone(to, false);

			EnterZone();

			StartCoroutine(UnloadAssets(status));
		}

		public void TransitionZone(Zone zone, SpawnPoint spawnPoint, Transition transition)
		{
			StartCoroutine(DoZoneTransition(zone, spawnPoint, transition));
		}

		private IEnumerator DoZoneTransition(Zone zone, SpawnPoint spawnPoint, Transition transition)
		{
			_transitionCount++;

			LeavingZone();

			var from = Player.Instance.Zone;
			var to = Zones[zone.Scene.Index];

			yield return TransitionManager.Instance.StartTransition(transition == null ? World.DefaultZoneTransition : transition, TransitionPhase.Out); // Don't null coalesce these

			var status = ChangeZone(to, true);

			while (!status.IsDone)
				yield return null;

			yield return UnloadAssets(status);
			yield return SpawnPlayer(to, spawnPoint);

			EnterZone();

			_transitionCount--;
		}

		private IEnumerator UnloadAssets(ZoneLoadStatus status)
		{
			while (!status.IsDone)
				yield return null;

			yield return Resources.UnloadUnusedAssets();
		}

		private IEnumerator SpawnPlayer(ZoneData zone, SpawnPoint spawnPoint)
		{
			if (spawnPoint.IsNamed)
				spawnPoint = zone.GetSpawnPoint(spawnPoint.Name);
			else
				spawnPoint = zone.SpawnPoints.Count > 0 ? zone.SpawnPoints.Values.First() : SpawnPoint.Default;

			Player.Instance.Mover.WarpToPosition(spawnPoint.Position, spawnPoint.Direction, spawnPoint.Layer);

			yield return TransitionManager.Instance.RunTransition(spawnPoint.Transition == null ? World.DefaultSpawnTransition : spawnPoint.Transition, TransitionPhase.In); // Don't null coalesce these

			if (spawnPoint.Move)
				Player.Instance.Mover.Move(spawnPoint.Direction);
		}

		private ZoneLoadStatus ChangeZone(ZoneData to, bool waitForConnections)
		{
			var from = Player.Instance.Zone;
			var loader = new ZoneLoadStatus { IsDone = false };

			if (from != null)
				LeaveZone(from, to);

			if (to != null)
			{
				if (to.State == ZoneState.Loaded && !waitForConnections)
					EnteringLoadedZone(loader, to, from);
				else
					StartCoroutine(EnteringUnloadedZone(loader, from, to, waitForConnections));
			}
			else
			{
				loader.IsDone = true;
			}

			return loader;
		}

		private void LeavingZone()
		{
			var zone = Player.Instance.Zone;
			if (zone != null)
				zone.Leaving();
		}

		private void LeaveZone(ZoneData from, ZoneData to)
		{
			from.Left(to);
			Player.Instance.Zone = null;
			SceneManager.SetActiveScene(gameObject.scene);

			UnloadConnections(from, to);

			_context.SetStore(nameof(Zone), null);
		}

		private void EnteringZone(ZoneData to, ZoneData from)
		{
			_context.SetStore(nameof(Zone), to);

			SceneManager.SetActiveScene(to.Zone.Scene.Scene);
			Player.Instance.Zone = to;
			to.Entering(from);
		}

		private void EnterZone()
		{
			var zone = Player.Instance.Zone;
			if (zone != null)
				zone.Entered();
		}

		#endregion

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => _variableStore.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => _variableStore.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => _variableStore.GetVariableNames();

		#endregion

		#region IVariableListener Implementation

		public void VariableChanged(int index, VariableValue value)
		{
			var variable = _variables.GetVariableName(index);

			foreach (var zone in LoadedZones)
				zone.VariableChanged(WorldListenerSource.World, variable);
		}

		#endregion

		#region Debug

		private void OnDrawGizmos()
		{
			foreach (var tile in _occupiedTiles)
			{
				if (tile.Value != CollisionLayer.None)
					DrawDebugBox(new Color(1.0f, 1.0f, 1.0f, 0.5f), Color.white, tile.Key);
			}

			foreach (var tile in _interactionTiles)
				DrawDebugBox(new Color(0.0f, 1.0f, 1.0f, 0.5f), Color.cyan, tile.Key);
		}

		private void DrawDebugBox(Color fill, Color outline, Vector2Int position)
		{
			var center = position + new Vector2(0.5f, 0.5f);
			var topLeft = position + Vector2.up;
			var topRight = position + Vector2.one;
			var bottomLeft = position + Vector2.zero;
			var bottomRight = position + Vector2.right;

			Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
			Gizmos.DrawCube(center, Vector3.one);
			Gizmos.color = Color.white;

			Gizmos.DrawLine(bottomLeft, topLeft);
			Gizmos.DrawLine(topRight, bottomRight);
			Gizmos.DrawLine(topLeft, topRight);
			Gizmos.DrawLine(bottomRight, bottomLeft);
		}

		#endregion
	}
}
