using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Scripted Sequence/Start Path", 20)]
	[HelpURL(MonsterRpg.DocumentationUrl + "start-path")]
	public class StartPath : PathNode
	{
		protected override Path GetPath(Mover mover)
		{
			var controller = mover.GetComponent<PathController>();
			return controller ? controller.Path : null;
		}
	}
}
