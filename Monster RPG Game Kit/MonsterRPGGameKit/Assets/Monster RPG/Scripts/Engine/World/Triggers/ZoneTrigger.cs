using PiRhoSoft.CompositionEngine;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class ZoneTrigger
	{
		public Zone TargetZone;

		[Tooltip("The spawn point to start at in the target zone")]
		public string TargetSpawn;

		[Tooltip("Whether this zone trigger starts a transition or not")]
		public bool HasTransition;

		[Tooltip("The Transition asset to use for this trigger (if null the default zone transition on World will be used)")]
		public Transition Transition;

		public void Enter()
		{
			if (TargetZone)
			{
				if (HasTransition)
				{
					Player.Instance.Mover.SkipNextUpdate();
					WorldManager.Instance.TransitionZone(TargetZone, new SpawnPoint { Name = TargetSpawn }, Transition);
				}
			}
		}

		public void Exit()
		{
			if (TargetZone)
			{
				if (!HasTransition)
					WorldManager.Instance.ChangeZone(TargetZone);
			}
		}
	}
}