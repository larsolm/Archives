using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class Path
	{
		public enum PathType
		{
			Loop,
			BackAndForth
		}

		[Serializable]
		public struct Node
		{
			[Tooltip("The position of the node.")] public Vector2Int Position;
			[Tooltip("How long to delay when this node is reached.")] public float MoveDelay;
		}

		[Tooltip("The type of path this mover will follow.")] public PathType Type;
		[Tooltip("The amount of timse to repeat this path (values less than 0 are infinite).")] public int RepeatCount = -1;
		[Tooltip("The nodes in this path.")] public Node[] Nodes = new Node[1];
	}

	[AddComponentMenu("Monster Maker/Move Controllers/Pathed Move Controller")]
	[RequireComponent(typeof(GridMover))] // TODO: THIS CAN PROBABLY BE ADAPTED TO FIT MULTIPLE TYPES OF CONTROLLERS
	public class PathedController : MoveController
	{
		[Tooltip("The instructions to run when a loop of the path has been completed.")] public Instruction OnLoopCompleted;
		[Tooltip("The instructions to run when the path is completed.")] public Instruction OnPathCompleted;
		[Tooltip("Whether or not the mover will use the pathfinding graph to get the best route to his next node.")] public bool UsePathfinding = true;
		[Tooltip("Whether or not the mover will attempt to find alternate routes if he becomes blocked along his path.")] public bool FindAlternateRoutes = true;

		[Header("Path")]
		[Tooltip("The path that the mover will move through. If the map in this scene has pathfinding it will automatically avoid walls and take the shortest path.")] public Path Path = new Path();

		private GridMover _gridMover;
		private Vector2Int _startingPosition;
		private Vector2Int _targetPosition;

		private int _nextIndex;
		private int _repeatCount;
		private int _direction;
		private float _movementDelay;
		private bool _running { get { return _direction != 0; } }

		private void Start()
		{
			_gridMover = Mover as GridMover;
			_gridMover.OnTileChanged += OnTileChanged;
			_startingPosition = _gridMover.CurrentGridPosition;
			_targetPosition = _startingPosition;
			_nextIndex = 1;
			_repeatCount = Path.RepeatCount;
			_direction = Path.Nodes.Length > 1 ? 1 : 0;
			_movementDelay = _running ? Path.Nodes[0].MoveDelay : 0.0f;

			FindPathToNextNode();
		}

		private void FixedUpdate()
		{
			_movementDelay -= Time.fixedDeltaTime;
			
			var horizontal = 0.0f;
			var vertical = 0.0f;

			if (_running && _movementDelay <= 0)
			{
				if (!_gridMover.Moving)
					FindPathToNextNode();

				var direction = _targetPosition - _gridMover.CurrentGridPosition;
				horizontal = Mathf.Clamp(direction.x, -1, 1);
				vertical = Mathf.Clamp(direction.y, -1, 1);
			}

			UpdateMover(horizontal, vertical);
		}

		private void OnTileChanged(Vector2Int previous, Vector2Int current)
		{
			var previousDirection = current - previous;
			var reachedNode = Path.Nodes[_nextIndex].Position + _startingPosition == current;

			if (reachedNode)
				NodeReached();

			if (reachedNode || FindAlternateRoutes)
				FindPathToNextNode();

			var currentDirection = _targetPosition - current;
			currentDirection.x = Mathf.Clamp(currentDirection.x, -1, 1);
			currentDirection.y = Mathf.Clamp(currentDirection.y, -1, 1);

			if (_movementDelay > 0 || currentDirection != previousDirection)
				_gridMover.DelayForFrame = true;
		}

		protected void LoopCompleted()
		{
			// INSTRUCTIONS?
		}

		protected void PathCompleted()
		{
			// INSTRUCTIONS?
		}

		private void NodeReached()
		{
			_movementDelay = Path.Nodes[_nextIndex].MoveDelay;
			_nextIndex += _direction;

			if (_nextIndex >= Path.Nodes.Length || _nextIndex < 0)
			{
				LoopCompleted();

				if (_repeatCount == 0)
				{
					PathCompleted();
					_direction = 0;
					return;
				}

				if (_repeatCount > 0)
					_repeatCount--;

				switch (Path.Type)
				{
					case Path.PathType.BackAndForth:
					{
						_nextIndex = _direction > 0 ? Path.Nodes.Length - 2 : 1;
						_direction *= -1;
						break;
					}
					case Path.PathType.Loop:
					{
						_nextIndex = _direction > 0 ? 0 : Path.Nodes.Length - 1;
						break;
					}
				}
			}
		}

		private void FindPathToNextNode()
		{
			if (_running)
			{
				var pathfinding = WorldManager.Instance.Zones[gameObject.scene.buildIndex].Pathfinding;
				var nextNode = Path.Nodes[_nextIndex];
				if (UsePathfinding && pathfinding)
				{
					var path = pathfinding.GetPath(_gridMover, _gridMover.CurrentGridPosition, _startingPosition + nextNode.Position, FindAlternateRoutes);

					if (path.Count > 1)
						_targetPosition = path[1];
				}
				else
				{
					_targetPosition = _startingPosition + nextNode.Position;
				}
			}
		}
	}
}
