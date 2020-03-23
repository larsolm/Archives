using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CreateAssetMenu(fileName = "Instructions", menuName = "Monster Maker/Composition/Instructions")]
	public class InstructionSet : ScriptableObject
	{
		public List<Instruction> Instructions = new List<Instruction>();
	}
}
