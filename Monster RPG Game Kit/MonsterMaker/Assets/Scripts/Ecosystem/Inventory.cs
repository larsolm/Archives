using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Ecosystem/Inventory")]
	public class Inventory : MonoBehaviour
	{
		public List<Item> Items = new List<Item>();
	}
}
