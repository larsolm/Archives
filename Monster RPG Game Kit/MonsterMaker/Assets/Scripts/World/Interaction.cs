using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/World/Interaction")]
	public class Interaction : MonoBehaviour
	{
		[Tooltip("The instructions to execute when the player interacts with this object")] public InstructionCaller Instructions;

		public void Interact(InstructionContext context)
		{
			if (Instructions != null && Instructions.Instruction != null)
				InstructionManager.Instance.StartCoroutine(Instructions.Run(context, this));
		}
	}
}
