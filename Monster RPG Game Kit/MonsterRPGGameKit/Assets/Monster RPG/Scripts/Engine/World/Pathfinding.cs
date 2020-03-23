using System;
using System.Collections.Generic;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(MapProperties))]
	[HelpURL(MonsterRpg.DocumentationUrl + "pathfinding")]
	[AddComponentMenu("PiRho Soft/World/Pathfinding")]
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

		private class PathOption : PriorityQueueNode
		{
			public Node Node;
			public int Cost;
		};

		[SerializeField] private Node[] _nodes;
		[SerializeField] private RectInt _bounds;
		
		private int _version = 0;

		void OnEnable()
		{
			if (_nodes == null)
				RegenerateNodes();
		}

		public void RegenerateNodes()
		{
			var map = GetComponent<MapProperties>();

			_bounds = map.GetBounds();
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

		public List<Vector2Int> GetPath(CollisionLayer layer, Vector2Int fromPosition, Vector2Int toPosition, bool respectOccupied)
		{
			_version++;

			// TODO: look all all these alocations!
			var path = new List<Vector2Int>();
			var start = GetNode(fromPosition - _bounds.min);
			var end = GetNode(toPosition - _bounds.min);

			if (start == null || end == null)
				return path;

			var currentLayer = layer;
			var last = start;
			var closest = GetDistance(fromPosition, toPosition);
			var neighbors = new Node[4];
			var map = GetComponent<MapProperties>();

			start.Version = _version;
			start.Previous = null;
			start.TotalCost = 0;

			var queue = new PriorityQueue<PathOption>(_bounds.width * _bounds.height);
			queue.Enqueue(new PathOption { Node = start, Cost = 0 }, 0);

			while (queue.Count != 0)
			{
				var current = queue.Dequeue();
				if (current.Node == end)
				{
					last = end;
					break;
				}

				var currentTile = map.GetTile(current.Node.Position);
				if (currentTile != null && currentTile.LayerChange != CollisionLayer.None)
					currentLayer = currentTile.LayerChange;

				GetNeighbors(current.Node, neighbors);

				foreach (var next in neighbors)
				{
					if (next == null)
						continue;

					var nextTile = map.GetTile(next.Position);
					var direction = Direction.GetDirection(next.Position - current.Node.Position);
					if ((respectOccupied && WorldManager.Instance.IsOccupied(next.Position, layer)) || (nextTile != null && (nextTile.IsEdge(direction) || nextTile.HasCollision(currentLayer) || (respectOccupied && WorldManager.Instance.IsOccupied(next.Position, nextTile.LayerChange)))))
						continue;

					var cost = current.Cost + 1;
					
					if (next.Version != _version || cost < next.TotalCost)
					{
						var distance = GetDistance(next.Position, toPosition);

						next.Version = _version;
						next.Previous = current.Node;
						next.TotalCost = cost;

						queue.Enqueue(new PathOption { Node = next, Cost = cost }, cost + distance);

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
