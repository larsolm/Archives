using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Ecosystem/Roster")]
	public class Roster : MonoBehaviour
	{
		[Tooltip("The ecosystem that this encounter object will access.")] public Ecosystem Ecosystem;
		[Tooltip("The list of creatures on this roster.")] public List<Creature> Creatures;
	}
}
