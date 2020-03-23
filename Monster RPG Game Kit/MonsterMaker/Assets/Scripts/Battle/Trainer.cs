using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[RequireComponent(typeof(Roster))]
	[AddComponentMenu("Monster Maker/Trainer")]
	public class Trainer : MonoBehaviour
	{
		public VariableStore Traits = new VariableStore();
	}
}
