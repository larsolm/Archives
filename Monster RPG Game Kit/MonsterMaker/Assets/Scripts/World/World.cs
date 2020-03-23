using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CreateAssetMenu(fileName = "World", menuName = "Monster Maker/World/World")]
	public class World : ScriptableObject
	{
		[Tooltip("The scenes that contain UI controls/listeners that are to be loaded at all times. For example, the pause menu and dialog boxes.")] public List<SceneReference> UIScenes;
		[Tooltip("The music to play by default when there is no other music being played.")] public AudioClip BackgroundMusic;
		[Tooltip("The layers that each zone can be in. Zones in layers that the player is not currently in will not be active.")] public List<string> MapLayers = new List<string>();

		[Tooltip("Variables for the world that cannot be changed in game.")] public VariableStore StaticState = new VariableStore();
		[Tooltip("Variables for the world that can be changed in game and are persisted when the game is saved.")] public VariableStore DefaultState = new VariableStore();

		public List<Zone> Zones = new List<Zone>();

		public Zone GetZone(string name)
		{
			foreach (var zone in Zones)
			{
				if (zone.name == name)
					return zone;
			}

			return Zones.Count == 0 ? null : Zones[0];
		}

		public Zone GetZoneBySceneIndex(int index)
		{
			foreach (var zone in Zones)
			{
				if (zone.Scene.Index == index)
					return zone;
			}

			return null;
		}
	}
}
