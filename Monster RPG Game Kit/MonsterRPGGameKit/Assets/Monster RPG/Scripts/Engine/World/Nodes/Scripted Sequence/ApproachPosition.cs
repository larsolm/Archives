using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Scripted Sequence/Approach Position", 1)]
	[HelpURL(MonsterRpg.DocumentationUrl + "approach-position")]
	public class ApproachPosition : ApproachNode
	{
		[Tooltip("The to walk to")]
		public Vector2Int Position;

		protected override Vector2Int GetTargetPosition(InstructionStore variables)
		{
			return Position;
		}
	}
}
