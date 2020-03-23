using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public enum ZoneStatus
	{
		Unloaded,
		Loading,
		Loaded,
		Unloading
	}

	public class ZoneData
	{
		public ZoneData(Zone zone)
		{
			Zone = zone;
			SceneIndex = zone.Scene.Index;
			Connections = new List<int>();
			SpawnPoints = new Dictionary<string, SpawnPoint>();
		}

		public ZoneStatus Status = ZoneStatus.Unloaded;

		public VariableStore PersistentState = new VariableStore();
		public VariableStore SessionState = new VariableStore();
		public VariableStore LoadedState = new VariableStore();
		public VariableStore ActiveState = new VariableStore();
		public VariableStore States = new VariableStore();

		public MapProperties Tilemap { get; private set; }
		public Pathfinding Pathfinding { get; private set; }
		public List<int> Connections { get; private set; }
		public Dictionary<string, SpawnPoint> SpawnPoints { get; private set; }

		public Zone Zone { get; private set; }
		public int SceneIndex { get; private set; }

		public SpawnPoint GetSpawnPoint(string name)
		{
			SpawnPoint spawn;

			if (SpawnPoints.Count == 0)
			{
				Debug.LogFormat("the MapProperties for zone {0} does not have any spawn points", Zone.name);

				spawn = new SpawnPoint();
				spawn.Direction.x = 0;
				spawn.Direction.y = -1.0f;
				spawn.Layer = CollisionLayer.One;

				if (Tilemap != null)
				{
					spawn.Position.x = (int)Tilemap.Bounds.center.x;
					spawn.Position.y = (int)Tilemap.Bounds.center.y;
				}
			}
			else if (!SpawnPoints.TryGetValue(name, out spawn))
			{
				var index = Random.Range(0, SpawnPoints.Count);
				spawn = SpawnPoints.ElementAt(index).Value;
			}

			return spawn;
		}

		public Rect GetArea()
		{
			return new Rect(Tilemap.Bounds.x, Tilemap.Bounds.y, Tilemap.Bounds.width, Tilemap.Bounds.height);
		}

		public void SetTilemap(MapProperties tilemap)
		{
			Tilemap = tilemap;

			if (Tilemap)
			{
				Pathfinding = Tilemap.GetComponent<Pathfinding>();
				Tilemap.AddConnections(Connections);
				Tilemap.AddSpawnPoints(SpawnPoints);
			}
			else
			{
				Pathfinding = null;
				Connections.Clear();
				SpawnPoints.Clear();
			}
		}
	}
}
