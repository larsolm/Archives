using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[CreateInstructionGraphNodeMenu("Scripted Sequence/Follow Path", 21)]
	[HelpURL(MonsterRpg.DocumentationUrl + "follow-path")]
	public class FollowPath : PathNode
	{
		[Tooltip("The path to move the target through")]
		public Path Path = new Path();
		
		protected override Path GetPath(Mover mover)
		{
			return Path;
		}
	}
}
