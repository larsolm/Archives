using System;
using System.Linq;
using System.Collections.Generic;
using PiRhoSoft.UtilityEngine;
using UnityEngine;
using PiRhoSoft.CompositionEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public enum ZoneState
	{
		Unloaded,
		Loading,
		Loaded,
		Unloading
	}

	[Serializable]
	public class ZoneSaveData
	{
		public string Name = "";
		public VariableList PersistentVariables = new VariableList();
		public List<NpcSaveData> Npcs = new List<NpcSaveData>();
	}

	[HelpURL(MonsterRpg.DocumentationUrl + "zone-data")]
	public class ZoneData : ScriptableObject, IVariableStore, IVariableListener
	{
		private const string _missingMapPropertiesError = "(WZDMMP) Failed to load zone {0}: the zone does not contain MapProperties";
		private const string _noSpawnPointsWarning = "(WZDNSP) The zone {0} does not have any spawn points";
		private const string _missingSpawnPointWarning = "(WZDMSP) The spawn point {0} could not be found in zone {1}";

		public Zone Zone;
		public int SceneIndex;

		public ZoneState State { get; internal set; }
		public bool IsActive { get; private set; }
		public bool IsEnabled { get; private set; }
		public WorldManager World { get; private set; }

		public MapProperties Properties { get; private set; }
		public Pathfinding Pathfinding { get; private set; }
		public List<int> Connections { get; private set; } = new List<int>();
		public Dictionary<string, SpawnPoint> SpawnPoints { get; private set; } = new Dictionary<string, SpawnPoint>();
		public List<WorldListener> Listeners { get; private set; } = new List<WorldListener>();
		public List<Npc> Npcs { get; private set; } = new List<Npc>();

		private VariableMap _variableMap;
		private VariableList _variables = new VariableList();
		private MappedVariableStore _variableStore = new MappedVariableStore();
		private Dictionary<string, NpcSaveData> _npcData = new Dictionary<string, NpcSaveData>();

		internal void Setup(WorldManager world, Zone zone, int index)
		{
			Zone = zone;

			SceneIndex = index;
			State = ZoneState.Unloaded;
			IsActive = false;
			IsEnabled = false;
			World = world;

			SetupVariables();
		}

		#region Variables

		public MappedVariableStore Variables => _variableStore;
		private static PropertyMap<ZoneData> _propertyMap;

		protected static VariableValue GetWorld(ZoneData zone) => VariableValue.Create(zone.World);
		protected static VariableValue GetPlayer(ZoneData zone) => VariableValue.Create(Player.Instance);
		protected static VariableValue GetName(ZoneData zone) => VariableValue.Create(zone.Zone.Name);
		protected static VariableValue GetState(ZoneData zone) => VariableValue.Create(zone.State.ToString());
		protected static VariableValue GetIsActive(ZoneData zone) => VariableValue.Create(zone.IsActive);

		protected void AddPropertiesToMap<ZoneDataType>(PropertyMap<ZoneDataType> map) where ZoneDataType : ZoneData
		{
			map.Add(nameof(World), GetWorld, null);
			map.Add(nameof(Player), GetPlayer, null);
			map.Add(nameof(Zone.Name), GetName, null);
			map.Add(nameof(State), GetState, null);
			map.Add(nameof(IsActive), GetIsActive, null);
		}

		protected virtual void SetupVariables()
		{
			if (_propertyMap == null)
			{
				_propertyMap = new PropertyMap<ZoneData>();
				AddPropertiesToMap(_propertyMap);
			}

			if (_variableMap == null || _variableMap.Version != Zone.Schema.Version)
			{
				_variableMap = new VariableMap(Zone.Schema.Version)
					.Add(_propertyMap)
					.Add(Zone.Schema);
			}

			_variableStore.Setup(_variableMap, new PropertyList<ZoneData>(this, _propertyMap), new VariableListener(this, _variables));
			_variables.Setup(Zone.Schema, this);
		}

		#endregion

		#region Map

		public SpawnPoint GetSpawnPoint(string name)
		{
			var spawn = SpawnPoint.Default;

			if (SpawnPoints.Count == 0)
			{
				Debug.LogWarningFormat(_noSpawnPointsWarning, Zone.name);
			}
			else if (!SpawnPoints.TryGetValue(name, out spawn))
			{
				Debug.LogWarningFormat(_missingSpawnPointWarning, name, Zone.name);
				spawn = SpawnPoints.Values.First();
			}

			return spawn;
		}

		#endregion

		#region Events

		internal bool Loaded()
		{
			var objects = Zone.Scene.Scene.GetRootGameObjects();

			foreach (var obj in objects)
			{
				if (Properties = obj.GetComponent<MapProperties>())
					break;
			}

			if (Properties != null)
			{
				ComponentHelper.GetComponentsInScene(Zone.Scene.Index, Npcs, true);
				Pathfinding = Properties.GetComponent<Pathfinding>();
				Properties.AddConnections(Connections);
				Properties.AddSpawnPoints(SpawnPoints);

				return true;
			}
			else
			{
				Debug.LogErrorFormat(_missingMapPropertiesError, Zone.name);
				return false;
			}
		}

		internal void Unloading()
		{
			_variables.ResetAvailability(Zone.ZoneLoadedAvailability);

			IsEnabled = false;

			Properties = null;
			Pathfinding = null;
			Connections.Clear();
			SpawnPoints.Clear();
			Npcs.Clear();
			Listeners.Clear();
		}

		internal void Enabled()
		{
			var objects = Zone.Scene.Scene.GetRootGameObjects();

			foreach (var obj in objects)
				obj.SetActive(true);

			IsEnabled = true;
		}

		internal void Disabled()
		{
			var objects = Zone.Scene.Scene.GetRootGameObjects();

			foreach (var obj in objects)
				obj.SetActive(false);

			IsEnabled = false;
		}

		internal void Leaving()
		{
			Trigger(Zone.ExitInstructions);
		}

		internal void Left(ZoneData to)
		{
			IsActive = false;

			_variables.ResetAvailability(Zone.ZoneActiveAvailability);

			if (AudioManager.Instance && Zone.BackgroundMusic != null && (to == null || Zone.BackgroundMusic != to.Zone.BackgroundMusic))
				AudioManager.Instance.Pop(1.0f, 1.0f, 1.0f);
		}

		internal void Entering(ZoneData from)
		{
			IsActive = true;

			if (AudioManager.Instance && Zone.BackgroundMusic != null && (from == null || Zone.BackgroundMusic != from.Zone.BackgroundMusic))
				AudioManager.Instance.Push(Zone.BackgroundMusic, 1.0f, 1.0f, 1.0f);
		}

		internal void Entered()
		{
			Trigger(Zone.EnterInstructions);
		}

		private void Trigger(InstructionCaller instructions)
		{
			if (instructions != null && instructions.Instruction != null)
				InstructionManager.Instance.RunInstruction(instructions, WorldManager.Instance.Context, this);
		}

		#endregion

		#region Persistence

		public virtual void Load(ZoneSaveData saveData)
		{
			_variables.LoadFrom(saveData.PersistentVariables, VariableDefinition.Saved);
			_npcData = saveData.Npcs.ToDictionary(npc => npc.Id);
		}

		public virtual void Save(ZoneSaveData saveData)
		{
			if (IsEnabled)
				PersistNpcData();

			_variables.SaveTo(saveData.PersistentVariables, VariableDefinition.Saved);
			saveData.Npcs = _npcData.Select(npc => npc.Value).ToList();
		}

		internal void RestoreNpcData()
		{
			foreach (var npc in Npcs)
			{
				_npcData.TryGetValue(npc.Guid, out NpcSaveData npcData);
				npc.Load(npcData);
			}
		}

		internal void PersistNpcData()
		{
			foreach (var npc in Npcs)
			{
				if (!_npcData.TryGetValue(npc.Guid, out NpcSaveData npcData))
				{
					npcData = new NpcSaveData { Id = npc.Guid.ToString() };
					_npcData.Add(npc.Guid, npcData);
				}

				npc.Save(npcData);
			}
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
			VariableChanged(WorldListenerSource.Zone, variable);
		}

		public void VariableChanged(WorldListenerSource source, string variable)
		{
			foreach (var listener in Listeners)
			{
				if (listener.gameObject.scene.buildIndex == SceneIndex)
					listener.OnVariableChanged(source, variable);
			}
		}

		#endregion
	}
}
