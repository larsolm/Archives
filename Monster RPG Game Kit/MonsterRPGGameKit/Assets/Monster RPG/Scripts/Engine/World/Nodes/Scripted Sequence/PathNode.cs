using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public abstract class PathNode : InstructionGraphNode
	{
		private const string _moverNotFoundWarning = "(WSSPNMNF) Unable to move mover through the path: the mover could not be found";
		private const string _pathNotFoundWarning = "(WSSPNPNF) Unable to move mover through the path: the path could not be found";
		private const string _repeatInfiniteWarning = "(WSSPNRI) Unable to wait for mover to move through the path: the path's repeat count was infinite";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("Whether to wait for the completion of the path")]
		public bool WaitForCompletion;

		private PathState _state = new PathState();

		protected abstract Path GetPath(Mover mover);

		public override Color NodeColor => Colors.Sequencing;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Mover mover)
			{
				var path = GetPath(mover);
				if (path != null)
				{
					_state.Start(path, mover);

					if (WaitForCompletion)
					{
						if (path.RepeatCount >= 0)
						{
							while (_state.Running)
								yield return null;
						}
						else
						{
							Debug.LogWarning(_repeatInfiniteWarning, this);
						}
					}
				}
				else
				{
					Debug.LogWarningFormat(this, _pathNotFoundWarning, Name);
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _moverNotFoundWarning, Name);
			}

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
