using PiRhoSoft.CompositionEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(ITrainer))]
	[HelpURL(MonsterRpg.DocumentationUrl + "battle-ai")]
	[AddComponentMenu("PiRho Soft/World/Battle Ai")]
	public class BattleAi : MonoBehaviour
	{
		public int MakeSelection(InstructionStore variables, List<VariableValue> values, string tag)
		{
			return SelectFirst(values, tag);
		}

		protected int SelectFirst(List<VariableValue> values, string tag)
		{
			return 0;
		}

		protected int SelectRandom(List<VariableValue> values, string tag)
		{
			return Random.Range(0, values.Count);
		}
	}
}
