using System;
using System.Collections;
using System.Collections.Generic;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class EncounterTable : DropTable<CreatureReference>
	{
	}

	[ExecuteInEditMode] // for Awake initialization of CreatureReferences
	[DisallowMultipleComponent]
	[HelpURL(MonsterRpg.DocumentationUrl + "encounter")]
	[AddComponentMenu("PiRho Soft/Ecosystem/Encounter")]
	public class Encounter : MonoBehaviour, ITrainer, IVariableReset
	{
		[Tooltip("The Instructions that run when a Creature is encountered")]
		public InstructionCaller Instructions = new InstructionCaller();

		[Tooltip("The percent chance to trigger the encounter when the player enters a tile that references this object")]
		[Range(0, 100.0f)]
		public float EncounterChance = 0.0f;

		[Tooltip("The encounters that can occur for this object")]
		public EncounterTable Encounters = new EncounterTable();

		private void Awake()
		{
			foreach (var encounter in Encounters.Values)
				encounter.Setup();
		}

		public virtual void Enter()
		{
			var chance = UnityEngine.Random.Range(0.0f, 100.0f);
			if (chance <= EncounterChance)
				DoEncounter();
		}

		protected virtual void DoEncounter()
		{
			var creature = PickCreature();
			StartCoroutine(RunEncounter(creature));
		}

		protected virtual Creature PickCreature()
		{
			var encounter = Encounters.PickValue();
			return encounter.CreateCreature(this);
		}

		protected virtual IEnumerator RunEncounter(Creature creature)
		{
			Roster.AddCreature(creature);

			if (Instructions != null && Instructions.Instruction != null)
				yield return Instructions.Execute(WorldManager.Instance.Context, this);

			Roster.DestroyCreatures();
		}

		#region ITrainer Implementation

		public string Name => Roster.Creatures.Count > 0 ? Roster.Creatures[0].Name : "";
		public Roster Roster { get; } = new Roster();

		#endregion

		#region IVariableReset Implementation

		public void ResetAvailability(string availability) => Roster.ResetAvailability(availability);
		public void ResetVariables(IList<string> traits) => Roster.ResetVariables(traits);

		#endregion

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => name == nameof(Roster) ? VariableValue.Create(Roster) : VariableValue.Empty;
		public SetVariableResult SetVariable(string name, VariableValue value) => name == nameof(Roster) ? SetVariableResult.ReadOnly : SetVariableResult.NotFound;
		public IEnumerable<string> GetVariableNames() => new List<string> { nameof(Roster) };

		#endregion
	}
}
