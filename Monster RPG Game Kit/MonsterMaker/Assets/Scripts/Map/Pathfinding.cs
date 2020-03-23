using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/World/Pathfinding")]
	[RequireComponent(typeof(MapProperties))]
	public class Pathfinding : MonoBehaviour
	{
		[Serializable]
		private class Node
		{
			public Vector2Int Index;
			public Vector2Int Position;

			[NonSerialized] public Node Previous = null;
			[NonSerialized] public int Version = 0;
			[NonSerialized] public int TotalCost = 0;
		};

		private struct PathOption : IComparable<PathOption>
		{
			public Node Node;
			public int Cost;
			public int Priority;

			public int CompareTo(PathOption other)
			{
				return other.Priority - Priority;
			}
		};

		[SerializeField] private Node[] _nodes;
		[SerializeField] private RectInt _bounds;
		
		private int _version = 0;

		void Awake()
		{
			if (_nodes == null)
				RegenerateNodes();
		}

		void OnEnable()
		{
			if(_nodes == null)
				RegenerateNodes();
		}

		public void RegenerateNodes()
		{
			var properties = GetComponent<MapProperties>();

			_bounds = properties.Bounds;
			_nodes = new Node[_bounds.width * _bounds.height];

			for (var x = 0; x < _bounds.width; x++)
			{
				for (var y = 0; y < _bounds.height; y++)
				{
					var position = new Vector2Int(x + _bounds.xMin, y + _bounds.yMin);
					var node = new Node
					{
						Index = new Vector2Int(x, y),
						Position = position,
					};

					_nodes[GetIndex(node.Index)] = node;
				}
			}
		}

		public List<Vector2Int> GetPath(Mover mover, Vector2Int fromPosition, Vector2Int toPosition, bool respectOccupied)
		{
			_version++;

			var path = new List<Vector2Int>();
			var start = GetNode(fromPosition - _bounds.min);
			var end = GetNode(toPosition - _bounds.min);

			if (start == null || end == null)
				return path;

			var currentLayer = mover.CollisionLayer;
			var last = start;
			var closest = GetDistance(fromPosition, toPosition);
			var neighbors = new Node[4];

			start.Version = _version;
			start.Previous = null;
			start.TotalCost = 0;

			var properties = GetComponent<MapProperties>();
			var queue = new PriorityQueue<PathOption>();
			queue.Push(new PathOption { Node = start, Cost = 0, Priority = 0 });

			while (!queue.Empty)
			{
				var current = queue.Pop();
				if (current.Node == end)
				{
					last = end;
					break;
				}

				var currentTile = properties.GetTile(current.Node.Position);
				if (currentTile != null && currentTile.CollisionLayerIncrement != CollisionLayer.None)
					currentLayer = currentTile.CollisionLayerIncrement;

				GetNeighbors(current.Node, neighbors);

				foreach (var next in neighbors)
				{
					if (next == null)
						continue;

					var nextTile = properties.GetTile(next.Position);
					if (nextTile != null && (nextTile.HasCollision(currentLayer) || (respectOccupied && nextTile.IsOccupied(currentLayer))))
						continue;

					var cost = current.Cost + 1;
					
					if (next.Version != _version || cost < next.TotalCost)
					{
						var distance = GetDistance(next.Position, toPosition);

						next.Version = _version;
						next.Previous = current.Node;
						next.TotalCost = cost;

						queue.Push(new PathOption { Node = next, Cost = cost, Priority = cost + distance });

						if (distance < closest)
						{
							closest = distance;
							last = next;
						}
					}
				}
			}

			while (last != null)
			{
				path.Add(last.Position);
				last = last.Previous;
			}

			path.Reverse();

			return path;
		}

		private Node GetNode(Vector2Int index)
		{
			if (_nodes == null || index.x < 0 || index.x >= _bounds.width || index.y < 0 || index.y >= _bounds.height)
				return null;

			return _nodes[GetIndex(index)];
		}

		private int GetIndex(Vector2Int position)
		{
			return position.x + (position.y * _bounds.width);
		}

		private void GetNeighbors(Node node, Node[] neighbors)
		{
			neighbors[0] = GetNode(node.Index + Vector2Int.left);
			neighbors[1] = GetNode(node.Index + Vector2Int.right);
			neighbors[2] = GetNode(node.Index + Vector2Int.up);
			neighbors[3] = GetNode(node.Index + Vector2Int.down);
		}

		private int GetDistance(Vector2Int from, Vector2Int to)
		{
			return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
		}
	}
}
