using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class CollisionLayerData : MonoBehaviour
	{
		public static int LayerCount = Enum.GetValues(typeof(CollisionLayer)).Length - 2;

		public CollisionLayer CollisionLayer = CollisionLayer.None;
	}
}
