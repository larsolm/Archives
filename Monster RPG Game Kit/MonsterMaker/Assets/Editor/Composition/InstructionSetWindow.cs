using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class InstructionTreeNode
	{
		// TODO: pool this

		public Instruction Instruction;
		public List<InstructionTreeNode> Parents;
		public List<InstructionTreeNode> Children;

		public InstructionTreeNode PreviousSibling;
		public InstructionTreeNode NextSibling;

		public int Id;
		public int Depth;
		public Rect Rect;
	}

	public class InstructionSetWindow : EditorWindow
	{
		public void Show(InstructionSet set)
		{
			titleContent = new GUIContent("Instructions");
			_set = set;

			BuildTree();

			Show();
		}

		void OnGUI()
		{
			using (var scroll = new EditorGUILayout.ScrollViewScope(_scroll, GUILayout.Width(position.width), GUILayout.Height(position.height)))
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					GUILayout.Label(_set.name, EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();

					if (GUILayout.Button("Refresh"))
						BuildTree();
				}

				var right = 0.0f;
				var bottom = 0.0f;

				BeginWindows();

				foreach (var node in _nodes)
				{
					node.Rect = GUI.Window(node.Id, node.Rect, WindowFunction, node.Instruction.name);
					if (GUI.Button(node.Rect, node.Instruction.name))
						Selection.activeObject = node.Instruction;

					if (node.Rect.xMax > right)
						right = node.Rect.xMax;

					if (node.Rect.yMax > bottom)
						bottom = node.Rect.yMax;
				}

				EndWindows();

				foreach (var node in _nodes)
				{
					if (node.Children != null)
					{
						var fromX = node.Rect.xMax;
						var fromY = node.Rect.center.y;

						foreach (var child in node.Children)
						{
							var toX = child.Rect.xMin;
							var toY = child.Rect.center.y;

							Handles.DrawLine(new Vector3(fromX, fromY), new Vector3(toX, toY));
						}
					}

					if (node.NextSibling != null)
					{
						var fromX = node.Rect.center.x;
						var fromY = node.Rect.yMax;
						var toX = node.NextSibling.Rect.center.x;
						var toY = node.NextSibling.Rect.yMin;

						Handles.DrawLine(new Vector3(fromX, fromY), new Vector3(toX, toY));
					}
				}

				GUILayoutUtility.GetRect(right, bottom);
				_scroll = scroll.scrollPosition;
			}
		}

		private InstructionSet _set;
		private List<InstructionTreeNode> _roots = new List<InstructionTreeNode>();
		private List<InstructionTreeNode> _nodes = new List<InstructionTreeNode>();
		private Vector2 _scroll;
		private int _nextId = 1;

		private void WindowFunction(int windowID)
		{
			if (GUI.Button(new Rect(10, 20, 100, 20), "Edit"))
			{
				var node = _nodes.FirstOrDefault(n => n.Id == windowID);
				Selection.activeObject = node.Instruction;
			}

			GUI.DragWindow();
		}

		private void BuildTree()
		{
			_roots.Clear();
			_nodes.Clear();

			for (var i = 0; i < _set.Instructions.Count; i++)
			{
				var instruction = _set.Instructions[i];
				var node = GetNode(instruction);
				var fields = instruction.GetType().GetFields().Where(field => typeof(Instruction).IsAssignableFrom(field.FieldType));
				var lists = instruction.GetType().GetFields().Where(field => field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>) && typeof(Instruction).IsAssignableFrom(field.FieldType.GetGenericArguments()[0]));

				foreach (var field in fields)
				{
					var child = field.GetValue(instruction) as Instruction;
					LinkInstruction(node, child);
				}

				foreach (var listField in lists)
				{
					InstructionTreeNode from = null;
					var list = listField.GetValue(instruction) as IList;

					for (var s = 0; s < list.Count; s++)
					{
						var sibling = list[s] as Instruction;

						if (s == 0)
							LinkInstruction(node, sibling);
						else
							LinkSibling(from, GetNode(sibling));

						from = GetNode(sibling);
					}
				}
			}

			for (var i = 0; i < _nodes.Count; i++)
			{
				var node = _nodes[i];
				if (node.Parents == null)
					_roots.Add(node);
			}

			var heights = new List<int>();

			foreach (var node in _nodes)
			{
				node.Depth = GetDepth(node);

				while (heights.Count <= node.Depth)
					heights.Add(0);

				var y = heights[node.Depth]++;
				node.Rect = GetRect(node.Depth, y);
			}
		}

		private int GetDepth(InstructionTreeNode node)
		{
			int biggestDepth = 0;

			if (node.Parents != null)
			{
				foreach (var parent in node.Parents)
				{
					var depth = GetDepth(parent) + 1;
					if (depth > biggestDepth)
						biggestDepth = depth;
				}
			}

			if (node.PreviousSibling != null)
			{
				if (node.PreviousSibling.Depth > biggestDepth)
					biggestDepth = node.PreviousSibling.Depth;
			}

			return biggestDepth;
		}

		private Rect GetRect(int x, int y)
		{
			var width = 250.0f;
			var height = 150.0f;

			return new Rect(50 + x * width, 50 + y * height, width - 50.0f, height - 20.0f);
		}

		private void LinkInstruction(InstructionTreeNode parent, Instruction instruction)
		{
			if (instruction != null)
			{
				var child = GetNode(instruction);

				if (parent.Children == null)
					parent.Children = new List<InstructionTreeNode>();

				parent.Children.Add(child);

				if (child.Parents == null)
					child.Parents = new List<InstructionTreeNode>();

				child.Parents.Add(parent);
			}
		}

		private void LinkSibling(InstructionTreeNode from, InstructionTreeNode to)
		{
			from.NextSibling = to;
			to.PreviousSibling = from;
		}

		private InstructionTreeNode GetNode(Instruction instruction)
		{
			var node = _nodes.Where(n => n.Instruction == instruction).FirstOrDefault();
			if (node == null)
			{
				node = new InstructionTreeNode{ Instruction = instruction, Id = _nextId++ };
				_nodes.Add(node);
			}

			return node;
		}
	}
}
