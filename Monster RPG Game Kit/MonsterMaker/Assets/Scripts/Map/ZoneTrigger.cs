using System;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public struct ZoneTrigger
	{
		public Zone TargetZone;
		public string TargetSpawn;

		public bool HasTransition;
		public Transition Transition;

		public bool Enter()
		{
			if (TargetZone)
			{
				if (HasTransition && Transition)
				{
					WorldManager.Instance.TransitionZone(TargetZone, TargetSpawn, Transition, false);
					return true;
				}
			}

			return false;
		}

		public void Leave()
		{
			if (TargetZone)
			{
				if (!HasTransition)
					WorldManager.Instance.UpdateCurrentZone();
			}
		}
	}
}
