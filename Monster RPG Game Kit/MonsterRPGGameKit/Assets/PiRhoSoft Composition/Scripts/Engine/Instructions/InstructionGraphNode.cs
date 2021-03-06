﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable] public class InstructionGraphNodeList : SerializedList<InstructionGraphNode> { }
	[Serializable] public class InstructionGraphNodeDictionary : SerializedDictionary<string, InstructionGraphNode> { }

	public class CreateInstructionGraphNodeMenuAttribute : Attribute
	{
		public string MenuName { get; private set; }
		public int Order { get; private set; }

		public CreateInstructionGraphNodeMenuAttribute(string menuName, int order = 0)
		{
			MenuName = menuName;
			Order = order;
		}
	}

	public interface ISequenceNode
	{
	}

	public interface ILoopNode
	{
	}

	public abstract class InstructionGraphNode : ScriptableObject
	{
		public static class Colors
		{
			public static readonly Color Start = new Color(0.1f, 0.1f, 0.1f);
			public static readonly Color Default = new Color(0.35f, 0.35f, 0.35f);
			public static readonly Color ExecutionLight = new Color(0.45f, 0.45f, 0.0f);
			public static readonly Color ExecutionDark = new Color(0.25f, 0.25f, 0.0f);
			public static readonly Color Animation = new Color(0.35f, 0.0f, 0.35f);
			public static readonly Color Sequence = new Color(0.5f, 0.2f, 0.2f);
			public static readonly Color Loop = new Color(0.35f, 0.1f, 0.1f);
			public static readonly Color Branch = new Color(0.2f, 0.1f, 0.1f);
			public static readonly Color Break = new Color(0.1f, 0.05f, 0.05f);
			public static readonly Color Sequencing = new Color(0.0f, 0.35f, 0.0f);
			public static readonly Color SequencingLight = new Color(0.0f, 0.45f, 0.0f);
			public static readonly Color SequencingDark = new Color(0.0f, 0.25f, 0.0f);
			public static readonly Color Interface = new Color(0.0f, 0.0f, 0.35f);
			public static readonly Color InterfaceLight = new Color(0.0f, 0.0f, 0.45f);
			public static readonly Color InterfaceDark = new Color(0.0f, 0.0f, 0.25f);
			public static readonly Color InterfaceCyan = new Color(0.0f, 0.3f, 0.5f);
			public static readonly Color InterfaceTeal = new Color(0.0f, 0.5f, 0.3f);
		}

		private const string _missingThisWarning = "(CCNMT) unable to find variable {0} for instruction graph node {1}";
		private const string _invalidThisWarning = "(CCNIT) variable {0} for instruction graph node {1} must be an object or store";
		private const string _missingKeyError = "(CCNMK) failed to set target: unable to find key {0} for instruction graph node {1}";
		private const string _missingIndexError = "(CCNMI) failed to set target: index {0} is out of range for instruction graph node {1}";
		private const string _missingFieldError = "(CCNMF) failed to set target: unable to find field {0} for instruction graph node {1}";

		[Tooltip("The name of the node")]
		[AssetName]
		public string Name;

		[Tooltip("The variable to use as input for operations on this node")]
		public VariableReference This = new VariableReference("this");

		public virtual void GetInputs(List<VariableDefinition> inputs) { }
		public virtual void GetOutputs(List<VariableDefinition> outputs) { }

		protected abstract IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration);

		public IEnumerator Run(InstructionGraph graph, InstructionStore variables, int executionIndex)
		{
			if (This.IsAssigned)
			{
				var value = This.GetValue(variables);

				if (value.Type == VariableType.Empty)
				{
					Debug.LogWarningFormat(_missingThisWarning, This, name);
				}
				else if (value.Type != VariableType.Null)
				{
					// null is a valid this object, but primitive types are not

					if (value.RawObject == null)
						Debug.LogWarningFormat(_invalidThisWarning, This, name);
					else
						variables.ChangeThis(value.RawObject);
				}
			}

			yield return Run_(graph, variables, executionIndex);
		}

		#region Editor Interface

		[HideInInspector] public Vector2 GraphPosition;
		[HideInInspector] public bool IsBreakpoint = false;

		public virtual Color NodeColor => Colors.Default;

		public class NodeData
		{
			public const float Width = 256.0f;
			public const float HeaderHeight = 22.0f;
			public const float LineHeight = 18.0f;
			public const float FooterHeight = 2.0f;

			private float _innerHeight = 0.0f;

			public InstructionGraphNode Node { get; private set; }
			public Rect Bounds { get; private set; }
			public List<ConnectionData> Connections = new List<ConnectionData>();

			public Vector2 Position
			{
				get
				{
					return Node.GraphPosition;
				}
				set
				{
					Node.GraphPosition = value;
					UpdateBounds();
				}
			}

			public float InnerHeight
			{
				get
				{
					return _innerHeight;
				}
				set
				{
					_innerHeight = value;
					UpdateBounds();
				}
			}

			public NodeData(InstructionGraphNode node)
			{
				Node = node;
				UpdateBounds();
			}

			public void ClearConnections()
			{
				Connections.Clear();
				UpdateBounds();
			}

			public void AddConnections(object obj)
			{
				var fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

				foreach (var field in fields)
				{
					if (field.FieldType == typeof(InstructionGraphNode))
					{
						var value = field.GetValue(obj) as InstructionGraphNode;
						AddConnection(field.Name, value);
					}
					else if (field.FieldType == typeof(InstructionGraphNodeDictionary))
					{
						var dictionary = field.GetValue(obj) as InstructionGraphNodeDictionary;

						foreach (var value in dictionary)
							AddConnection(field.Name, value.Key, value.Value);
					}
					else if (field.FieldType == typeof(InstructionGraphNodeList))
					{
						var list = field.GetValue(obj) as InstructionGraphNodeList;

						for (var i = 0; i < list.Count; i++)
							AddConnection(field.Name, i, list[i]);
					}
				}
			}

			public void AddConnection(string name, InstructionGraphNode to)
			{
				Connections.Add(new ConnectionData(name, null, -1, Node, to, Connections.Count));
				UpdateBounds();
			}

			public void AddConnection(string name, string key, InstructionGraphNode to)
			{
				Connections.Add(new ConnectionData(name, key, -1, Node, to, Connections.Count));
				UpdateBounds();
			}

			public void AddConnection(string name, int index, InstructionGraphNode to)
			{
				Connections.Add(new ConnectionData(name, null, index, Node, to, Connections.Count));
				UpdateBounds();
			}

			private void UpdateBounds()
			{
				Bounds = new Rect(Position, new Vector2(Width, Connections.Count * LineHeight + HeaderHeight + FooterHeight + _innerHeight));
			}
		}

		public class ConnectionData
		{
			public string Field { get; private set; }
			public string FieldKey { get; private set; }
			public int FieldIndex { get; private set; }

			public InstructionGraphNode From { get; private set; }
			public int FromIndex { get; private set; }
			public InstructionGraphNode To { get; private set; }
			public NodeData Target { get; private set; }

			public string Name { get; private set; }

			public static bool operator ==(ConnectionData left, ConnectionData right)
			{
				// need to override since connections are rebuilt for the selected node causing reference comparison
				// to return false

				if (ReferenceEquals(left, null))
					return ReferenceEquals(right, null);
				else if (ReferenceEquals(right, null))
					return false;
				else
					return left.From == right.From && left.FromIndex == right.FromIndex;
			}

			public static bool operator !=(ConnectionData left, ConnectionData right)
			{
				return !(left == right);
			}

			public override bool Equals(object obj)
			{
				if (obj is ConnectionData other)
					return this == other;

				return false;
			}

			public override int GetHashCode()
			{
				// not needed but Visual Studio warns without it

				var hashCode = -2083501448;
				hashCode = hashCode * -1521134295 + EqualityComparer<InstructionGraphNode>.Default.GetHashCode(From);
				hashCode = hashCode * -1521134295 + FromIndex.GetHashCode();
				return hashCode;
			}

			public ConnectionData(string field, string key, int index, InstructionGraphNode from, InstructionGraphNode to, int fromIndex)
			{
				Field = field;
				FieldKey = key;
				FieldIndex = index;
				From = from;
				FromIndex = fromIndex;
				To = to;

				if (index >= 0)
					Name = string.Format("{0} {1}", field, index);
				else if (!string.IsNullOrEmpty(key))
					Name = string.Format("{0} {1}", field, key);
				else
					Name = field;
			}

			public void SetTarget(NodeData target)
			{
				Target = target;
			}

			public void ChangeTarget(NodeData target)
			{
				To = target?.Node;
				Target = target;

				From.SetConnection(this, To);
			}

			public void ApplyConnection(object obj, InstructionGraphNode target)
			{
				var field = obj.GetType().GetField(Field, BindingFlags.Instance | BindingFlags.Public);

				if (field != null)
				{
					if (field.FieldType == typeof(InstructionGraphNode))
					{
						field.SetValue(obj, target);
					}
					else if (field.FieldType == typeof(InstructionGraphNodeDictionary))
					{
						var dictionary = field.GetValue(obj) as InstructionGraphNodeDictionary;

						if (dictionary.ContainsKey(FieldKey))
							dictionary[FieldKey] = target;
						else
							Debug.LogErrorFormat(_missingKeyError, Field, target.name);
					}
					else if (field.FieldType == typeof(InstructionGraphNodeList))
					{
						var list = field.GetValue(obj) as InstructionGraphNodeList;

						if (FieldIndex >= 0 && FieldIndex < list.Count)
							list[FieldIndex] = target;
						else
							Debug.LogErrorFormat(_missingIndexError, Field, Target.Node.name);
					}
					else
					{
						Debug.LogErrorFormat(_missingFieldError, Field, Target.Node.name);
					}
				}
				else
				{
					Debug.LogErrorFormat(_missingFieldError, Field, Target.Node.name);
				}
			}
		}

		public virtual void GetConnections(NodeData data)
		{
			data.AddConnections(this);
		}

		public virtual void SetConnection(ConnectionData connection, InstructionGraphNode target)
		{
			connection.ApplyConnection(this, target);
		}

		#endregion
	}
}
