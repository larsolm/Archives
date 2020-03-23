using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CreateAssetMenu(fileName = "Ecosystem", menuName = "Monster Maker/Ecosystem/Ecosystem")]
	public class Ecosystem : ScriptableObject
	{
		public List<Species> Species = new List<Species>();
		public List<Ability> Abilities = new List<Ability>();

		[Tooltip("The instructions to run when generating a creature from a species.")] public Instruction CreatureGenerationInstructions;
		[Tooltip("The instructions to run when creature traits are changed.")] [SerializeField] public InstructionDictionary CreatureUpdateInstructions = new InstructionDictionary();
	}
}
