using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class Roster : SerializedList<CreatureReference>, IVariableReset, IIndexedVariableStore
	{
		public List<Creature> Creatures { get; private set; }

		public void Setup()
		{
			foreach (var reference in this)
				reference.Setup();
		}

		public void CreateCreatures(ITrainer trainer)
		{
			Creatures = new List<Creature>(base.Count);

			foreach (var reference in this)
				Creatures.Add(reference.CreateCreature(trainer));
		}

		public void DestroyCreatures()
		{
			if (Creatures != null)
			{
				foreach (var creature in Creatures)
				{
					creature.Teardown();
					Object.Destroy(creature);
				}

				Creatures.Clear();
			}
		}

		public void AddCreature(Creature creature)
		{
			Creatures.Add(creature);
		}

		public void RemoveCreature(Creature creature)
		{
			TakeCreature(creature);
			creature.Teardown();
			Object.Destroy(creature);
		}

		public void TakeCreature(Creature creature)
		{
			for (var i = 0; i < Creatures.Count; i++)
			{
				if (Creatures[i] == creature)
				{
					Creatures.RemoveAt(i);
					break;
				}
			}
		}

		#region IVariableReset Implementation

		public void ResetAvailability(string availability)
		{
			foreach (var creature in Creatures)
				creature.ResetAvailability(availability);
		}

		public void ResetVariables(IList<string> traits)
		{
			foreach (var creature in Creatures)
				creature.ResetVariables(traits);
		}

		#endregion

		#region IIndexedVariableStore Implementation

		public IVariableStore GetItem(int index) => index >= 0 && index < Creatures.Count ? Creatures[index] : null;
		public VariableValue GetVariable(string name) => IndexedVariableStore.GetVariable(this, name);
		public SetVariableResult SetVariable(string name, VariableValue value) => IndexedVariableStore.SetVariable(this, name, value);
		public IEnumerable<string> GetVariableNames() => IndexedVariableStore.GetVariableNames(this);

		#endregion
	}
}
