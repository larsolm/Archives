using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[VariableSource("Ecosystem")]
	public enum EcosystemVariableSource
	{
		Species,
		Creature
	}

	public class EcosystemInstructionContext : InstructionContext
	{
		public EcosystemInstructionContext(Object Owner, string name, VariableStore variables) : base(Owner, name, variables)
		{
		}

		protected override VariableStore GetCustomStore(VariableReference variable)
		{
			var creature = Owner as Creature;

			if (creature != null)
			{
				switch (variable.CustomSource)
				{
					case "Ecosystem": return variable.CustomIndex == 0 ? creature.Species.Traits : creature.Traits;
					case "Species": return creature.Species.Traits;
					case "Creature": return creature.Traits;
				}
			}

			return null;
		}

		protected override List<InstructionListener> GetListeners(string category)
		{
			return _listeners;
		}

		private List<InstructionListener> _listeners = new List<InstructionListener>();
	}
}
