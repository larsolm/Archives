using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public struct EncounterType
	{
		public Species Species;
		[Tooltip("The variables that this encounter will have.")] [VariableAccess(AllowAdd = false, AllowRemove = false)] public VariableStore Variables;
	}

	[AddComponentMenu("Monster Maker/World/Encounter")]
	public class Encounter : MonoBehaviour
	{
		[Serializable] public class EncounterTable : DropTable<EncounterType> { };

		[Tooltip("The ecosystem that this encounter object will access.")] public Ecosystem Ecosystem;
		[Tooltip("The instructions to run when this encounter occurs.")] public Instruction Instructions;
		[Tooltip("The instructions to run when generating a creature for this encounter.")] public Instruction GenerationInstructions;
		[Tooltip("The percent chance to encounter a creature when the player enters this square.")] [Range(0, 100.0f)] public float EncounterChance = 0.0f;
		[Tooltip("The encounters that can occur for this object.")] public EncounterTable Encounters = new EncounterTable();

		public bool Enter()
		{
			var chance = UnityEngine.Random.Range(0.0f, 100.0f);
			if (chance <= EncounterChance)
			{
				InitiateEncounter();
				return true;
			}

			return false;
		}

		private void InitiateEncounter()
		{
			//var encounter = Encounters.PickValue();
			//var creature = encounter.Species.CreateCreature();
			//InstructionManager.Instance.Run(Instructions, this);
		}
	}
}
