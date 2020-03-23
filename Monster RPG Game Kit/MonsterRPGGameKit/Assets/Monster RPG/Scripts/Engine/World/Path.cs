using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class Path
	{
		[Serializable]
		public struct NodeData
		{
			[Tooltip("The position of this node")]
			public Vector2Int Position;

			[Tooltip("The direction to face when stopped at this node")]
			[EnumButtons]
			public MovementDirection Direction;

			[Tooltip("How long to wait at this node before continuing")]
			[Minimum(0.0f)]
			public float Delay;
		}

		public enum PathType
		{
			Loop,
			BackAndForth
		}

		[Tooltip("The type of path to will follow")]
		[EnumButtons]
		public PathType Type;

		[Tooltip("The amount of times to repeat this path (values less than 0 are infinite)")]
		[Minimum(-1)]
		public int RepeatCount = -1;

		[Tooltip("Whether to use absolute positioning in the world or positioning relative to the mover")]
		public bool UseAbsolutePositioning = false;

		[Tooltip("Whether or not the mover will use the pathfinding graph to get the best route to his next node")]
		public bool UsePathfinding = true;

		[Tooltip("Whether or not the mover will attempt to find alternate routes if he becomes blocked while traveling the path")]
		public bool FindAlternateRoutes = true;

		[Tooltip("The nodes in this path")]
		public NodeData[] Nodes = new NodeData[1];
	}

	public class PathState
	{
		private Path _path;
		private Mover _mover;
		private Controller _controller;
		private Vector2Int _startingPosition;
		private Vector2Int _targetPosition;

		private int _nextIndex;
		private int _repeatCount;
		private int _direction;
		private float _delay;

		private Coroutine _coroutine;

		public bool Running => _direction != 0;

		public void Start(Path path, Mover mover, Controller controller = null)
		{
			_path = path;
			_controller = controller;
			_mover = mover;
			_mover.OnTileChanged += OnTileChanged;
			_startingPosition = _mover.CurrentGridPosition;
			_targetPosition = _startingPosition;
			_nextIndex = 1;
			_repeatCount = _path.RepeatCount;
			_direction = _path.Nodes.Length > 1 ? 1 : 0;
			_delay = Running ? _path.Nodes[0].Delay : 0.0f;

			FindPathToNextNode();

			_coroutine = _mover.StartCoroutine(Run());
		}

		public void Stop()
		{
			_mover.OnTileChanged -= OnTileChanged;
			_mover.StopCoroutine(_coroutine);
			_coroutine = null;
			_direction = 0;
		}

		private IEnumerator Run()
		{
			while (true)
			{
				var horizontal = 0.0f;
				var vertical = 0.0f;

				if (Running && (_controller == null || !_controller.IsFrozen))
				{
					_delay -= Time.fixedDeltaTime;

					if (_delay <= 0)
					{
						if (!_mover.Moving)
							FindPathToNextNode();

						var direction = _targetPosition - _mover.CurrentGridPosition;
						horizontal = Mathf.Clamp(direction.x, -1, 1);
						vertical = Mathf.Clamp(direction.y, -1, 1);
					}
				}

				_mover.UpdateMove(horizontal, vertical);

				yield return new WaitForFixedUpdate();
			}
		}

		private void OnTileChanged(Vector2Int previous, Vector2Int current)
		{
			if (Running)
			{
				var previousDirection = current - previous;
				var reachedNode = GetNodePosition(_nextIndex, _startingPosition) == current;

				if (reachedNode)
					NodeReached();

				if (reachedNode || _path.UsePathfinding)
					FindPathToNextNode();

				var currentDirection = _targetPosition - current;
				currentDirection.x = Mathf.Clamp(currentDirection.x, -1, 1);
				currentDirection.y = Mathf.Clamp(currentDirection.y, -1, 1);

				if (_delay > 0 || currentDirection != previousDirection)
					_mover.SkipNextUpdate();
			}
		}

		private void NodeReached()
		{
			var node = _path.Nodes[_nextIndex];

			_delay = node.Delay;
			_mover.FaceDirection(node.Direction);

			_nextIndex += _direction;

			if (_nextIndex >= _path.Nodes.Length || _nextIndex < 0)
			{
				if (_repeatCount == 0)
				{
					Stop();
					return;
				}

				if (_repeatCount > 0)
					_repeatCount--;

				switch (_path.Type)
				{
					case Path.PathType.BackAndForth:
					{
						_nextIndex = _direction > 0 ? _path.Nodes.Length - 2 : 1;
						_direction *= -1;
						break;
					}
					case Path.PathType.Loop:
					{
						_nextIndex = _direction > 0 ? 0 : _path.Nodes.Length - 1;
						break;
					}
				}
			}
		}

		private void FindPathToNextNode()
		{
			if (Running)
			{
				var pathfinding = WorldManager.Instance.Zones[_mover.gameObject.scene.buildIndex].Pathfinding;
				var nodePosition = GetNodePosition(_nextIndex, _startingPosition);
				if (_path.UsePathfinding && pathfinding)
				{
					var path = pathfinding.GetPath(_mover.MovementLayer, _mover.CurrentGridPosition, nodePosition, _path.FindAlternateRoutes);

					if (path.Count > 1)
						_targetPosition = path[1];
				}
				else
				{
					_targetPosition = nodePosition;
				}
			}
		}

		private Vector2Int GetNodePosition(int index, Vector2Int startPosition)
		{
			var position = index < 0 || index >= _path.Nodes.Length ? Vector2Int.zero : _path.Nodes[index].Position;
			return _path.UseAbsolutePositioning && index != 0 ? position : position + startPosition;
		}

		#region Persistence

		public void Load(string saveData)
		{
			var elements = saveData.Split('|');

			if (elements.Length == 8)
			{
				if (int.TryParse(elements[0], out int sx)) _startingPosition.x = sx;
				if (int.TryParse(elements[1], out int sy)) _startingPosition.y = sy;
				if (int.TryParse(elements[2], out int tx)) _targetPosition.x = tx;
				if (int.TryParse(elements[3], out int ty)) _targetPosition.y = ty;
				if (int.TryParse(elements[4], out int ni)) _nextIndex = ni;
				if (int.TryParse(elements[5], out int rc)) _repeatCount = rc;
				if (int.TryParse(elements[6], out int di)) _direction = di;
				if (float.TryParse(elements[7], out float de)) _delay = de;

				FindPathToNextNode();
			}
		}

		public string Save()
		{
			return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", _startingPosition.x, _startingPosition.y, _targetPosition.x, _targetPosition.y, _nextIndex, _repeatCount, _direction, _delay);
		}

		#endregion
	}
}
