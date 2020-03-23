using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/UI/Selection Item")]
	public class SelectionItem : MonoBehaviour
	{
		public virtual void Create(int index, object data)
		{
		}

		public virtual void Focus()
		{
		}

		public virtual void Blur()
		{
		}
	}
}
