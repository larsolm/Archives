using PiRhoSoft.CompositionEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public abstract class ApproachNode : InstructionGraphNode
	{
		private const string _moverNotFoundWarning = "(WSSANMNF) Unable to make mover approach the targe for {0}: the given variables must be a Mover";
		private const string _noPathfindingWarning = "(WSSANNP) Unable to make mover approach the target for {0}: UsePathfinding is specified but the map does not have a pathfinding component";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("Whether to wait for the completion of the movement")]
		public bool WaitForCompletion;

		[Tooltip("Whether to use pathfinding to find a valid path to the position")]
		public bool UsePathfinding;

		public override Color NodeColor => Colors.Sequencing;

		private PathState _state = new PathState();
		private Path _path = new Path
		{
			Nodes = new Path.NodeData[2],
			UseAbsolutePositioning = true,
			FindAlternateRoutes = true,
			RepeatCount = 0
		};

		protected abstract Vector2Int GetTargetPosition(InstructionStore variables);

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Mover mover)
			{
				var source = mover.CurrentGridPosition;
				var target = GetTargetPosition(variables);

				_path.UsePathfinding = UsePathfinding;
				_path.Nodes[0].Position = Vector2Int.zero;

				if (UsePathfinding)
				{
					var pathfinding = WorldManager.Instance.Zones[mover.gameObject.scene.buildIndex].Pathfinding;
					if (pathfinding)
					{
						var path = pathfinding.GetPath(mover.MovementLayer, source, target, true);
						if (path.Count > 1)
						{
							target = path[path.Count - 1];
						}
						else
						{
							graph.GoTo(Next, variables.This, nameof(Next));
							yield break;
						}
					}
					else
					{
						Debug.LogWarningFormat(this, _noPathfindingWarning, Name);
						graph.GoTo(Next, variables.This, nameof(Next));
						yield break;
					}
				}
				else
				{
					var left = target + Vector2Int.left;
					var right = target + Vector2Int.right;
					var up = target + Vector2Int.up;
					var down = target + Vector2Int.down;

					if (left == source || right == source || up == source || down == source)
					{
						graph.GoTo(Next, variables.This, nameof(Next));
						yield break;
					}

					var dLeft = left - source;
					var dRight = right - source;
					var dUp = up - source;
					var dDown = down - source;

					var mLeft = Mathf.Abs(dLeft.x) + Mathf.Abs(dLeft.y);
					var mRight = Mathf.Abs(dRight.x) + Mathf.Abs(dRight.y);
					var mUp = Mathf.Abs(dUp.x) + Mathf.Abs(dUp.y);
					var mDown = Mathf.Abs(dDown.x) + Mathf.Abs(dDown.y);

					var least = Mathf.Min(mLeft, mRight, mUp, mDown);

					if (mLeft == least)
						target = left;
					else if (mRight == least)
						target = right;
					else if (mUp == least)
						target = up;
					else if (mDown == least)
						target = down;
				}

				_path.Nodes[1].Position = target;
				_state.Start(_path, mover);

				if (WaitForCompletion)
				{
					while (_state.Running)
						yield return null;
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
