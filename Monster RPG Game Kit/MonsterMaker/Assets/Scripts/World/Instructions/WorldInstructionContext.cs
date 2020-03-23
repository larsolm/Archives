using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[VariableSource("World")]
	public enum WorldVariableSource
	{
		Static,
		Persistent,
		Session
	}

	[VariableSource("Current Zone")]
	public enum CurrentZoneVariableSource
	{
		Static,
		Persistent,
		Session,
		Loaded,
		Active
	}

	[VariableSource("My Zone")]
	public enum MyZoneVariableSource
	{
		Static,
		Persistent,
		Session,
		Loaded,
		Active
	}

	public class WorldInstructionContext : InstructionContext
	{
		public WorldInstructionContext(Object owner, string name, VariableStore variables) : base(owner, name, variables)
		{
		}

		protected override VariableStore GetCustomStore(VariableReference variable)
		{
			var index = variable.CustomIndex;

			if (variable.CustomSource.EndsWith("Static")) index = 0;
			else if (variable.CustomSource.EndsWith("Persistent")) index = 1;
			else if (variable.CustomSource.EndsWith("Session")) index = 2;
			else if (variable.CustomSource.EndsWith("Loaded")) index = 3;
			else if (variable.CustomSource.EndsWith("Active")) index = 4;

			if (variable.CustomSource.StartsWith("World")) return GetVariableStore((WorldVariableSource)index);
			if (variable.CustomSource.StartsWith("Current Zone")) return GetVariableStore((CurrentZoneVariableSource)index);
			if (variable.CustomSource.StartsWith("My Zone")) return GetVariableStore((MyZoneVariableSource)index);

			return null;
		}

		protected override List<InstructionListener> GetListeners(string category)
		{
			return InstructionManager.Instance.GetListeners(category);
		}

		private VariableStore GetVariableStore(WorldVariableSource source)
		{
			switch (source)
			{
				case WorldVariableSource.Static: return WorldManager.Instance.World.StaticState;
				case WorldVariableSource.Persistent: return WorldManager.Instance.PersistentState;
				case WorldVariableSource.Session: return WorldManager.Instance.SessionState;
			}

			return null;
		}

		private VariableStore GetVariableStore(CurrentZoneVariableSource source)
		{
			var zone = WorldManager.Instance.CurrentZone;

			if (zone != null)
			{
				switch (source)
				{
					case CurrentZoneVariableSource.Static: return zone.Zone.StaticState;
					case CurrentZoneVariableSource.Persistent: return zone.PersistentState;
					case CurrentZoneVariableSource.Session: return zone.SessionState;
					case CurrentZoneVariableSource.Loaded: return zone.LoadedState;
					case CurrentZoneVariableSource.Active: return zone.ActiveState;
				}
			}

			return null;
		}

		private VariableStore GetVariableStore(MyZoneVariableSource source)
		{
			var zone = WorldManager.Instance.GetZone(Owner);

			if (zone != null)
			{
				switch (source)
				{
					case MyZoneVariableSource.Static: return zone.Zone.StaticState;
					case MyZoneVariableSource.Persistent: return zone.PersistentState;
					case MyZoneVariableSource.Session: return zone.SessionState;
					case MyZoneVariableSource.Loaded: return zone.LoadedState;
					case MyZoneVariableSource.Active: return zone.ActiveState;
				}
			}

			return null;
		}
	}
}
