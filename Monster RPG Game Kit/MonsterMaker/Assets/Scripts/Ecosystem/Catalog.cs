using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CreateAssetMenu(fileName = "Catalog", menuName = "Monster Maker/Ecosystem/Catalog")]
	public class Catalog : ScriptableObject
	{
		public List<Product> Products = new List<Product>();
	}
}
