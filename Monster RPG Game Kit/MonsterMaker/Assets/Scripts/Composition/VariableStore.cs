using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class VariableExistsException : Exception
	{
		public VariableExistsException(string name) : base(string.Format("The variable '{0}' already exists in the store", name))
		{
			Name = name;
		}

		public string Name { get; private set; }
	}

	public class VariableMissingException : Exception
	{
		public VariableMissingException(string name) : base(string.Format("The variable '{0}' does not exist in the store", name))
		{
			Name = name;
		}

		public string Name { get; private set; }
	}

	public class VariableOutOfRangeException : Exception
	{
		public VariableOutOfRangeException(int index, int count) : base(string.Format("The index {0} is out of range for the store of size {1}", index, count))
		{
			Index = index;
			Count = count;
		}

		public int Index { get; private set; }
		public int Count { get; private set; }
	}

	public class VariableTypeMismatchException : Exception
	{
		public VariableTypeMismatchException(string name, Type requestedType, VariableType actualType) : base(string.Format("The variable '{0}' of type {1} cannot be accessed as a {2}", name, requestedType.Name, actualType))
		{
			Name = name;
			RequestedType = requestedType;
			ActualType = actualType;
		}

		public string Name { get; private set; }
		public Type RequestedType { get; private set; }
		public VariableType ActualType { get; private set; }
	}

	public class VariableAccessException : Exception
	{
		public VariableAccessException(VariableAccess requested, VariableAccess available) : base(string.Format("{0} access required for VariableStore with access {1}", requested, available))
		{
			RequestedAccess = requested;
			AvailableAccess = available;
		}

		public VariableAccess RequestedAccess { get; private set; }
		public VariableAccess AvailableAccess { get; private set; }
	}

	public interface IVariableListener
	{
		void OnVariableAdded(VariableValue variable, object owner);
		void OnVariableChanged(VariableValue variable, object owner);
		void OnVariableRemoved(string name, object owner);
	}

	[Flags]
	public enum VariableAccess
	{
		Get = 0x1,
		Set = 0x2,
		Change = 0x4,
		Add = 0x8,
		Remove = 0x10,
		ReadOnly = Get,
		ReadAndWrite = Get | Set | Change,
		Full = ReadAndWrite | Add | Remove
	}

	[Serializable]
	public class VariableStore : ISerializationCallbackReceiver, IPoolable
	{
		[NonSerialized] public VariableAccess Access = VariableAccess.Full;
		public ReadOnlyCollection<VariableValue> Variables { get; private set; }

		[SerializeField] private List<VariableValue> _variables = new List<VariableValue>();
		private Dictionary<string, int> _variablesMap = new Dictionary<string, int>();
		private List<Listener> _listeners = new List<Listener>();

		public int Count { get { return _variables.Count; } }

		public VariableStore()
		{
			Variables = new ReadOnlyCollection<VariableValue>(_variables);
		}

		public bool HasAccess(VariableAccess access)
		{
			return (Access & access) != 0;
		}

		public void AddAccess(VariableAccess access)
		{
			Access |= access;
		}

		public void RestrictAccess(VariableAccess access)
		{
			Access &= ~access;
		}

		public VariableValue GetVariable(string name)
		{
			int index;
			if (_variablesMap.TryGetValue(name, out index))
				return GetVariable(index);

			return null;
		}

		public int GetIndex(string name)
		{
			int index;
			if (_variablesMap.TryGetValue(name, out index))
				return index;

			return -1;
		}

		public bool Exists(string name)
		{
			return GetVariable(name) != null;
		}

		public VariableType GetType(string name)
		{
			var variable = GetVariable(name);
			return variable != null ? variable.Type : VariableType.Empty;
		}

		public bool HasType<T>(string name)
		{
			var variable = GetVariable(name);
			return variable != null ? variable.HasType<T>() : false;
		}

		public bool HasType(string name, Type type)
		{
			var variable = GetVariable(name);
			return variable != null ? variable.HasType(type) : false;
		}

		public void Add<T>(string name, T value)
		{
			if (!HasAccess(VariableAccess.Add))
				throw new VariableAccessException(VariableAccess.Add, Access);

			var variable = GetVariable(name);

			if (variable != null)
				throw new VariableExistsException(name);

			DoAdd(VariableValue.Create(name, value));
		}

		public T Get<T>(string name)
		{
			if (!HasAccess(VariableAccess.Get))
				throw new VariableAccessException(VariableAccess.Get, Access);

			var variable = GetVariable(name);
			T value;

			if (variable == null)
				throw new VariableMissingException(name);

			if (!variable.TryGet(out value))
				throw new VariableTypeMismatchException(name, typeof(T), variable.Type);

			return value;
		}

		public void Set<T>(string name, T value)
		{
			if (!HasAccess(VariableAccess.Set))
				throw new VariableAccessException(VariableAccess.Set, Access);

			var variable = GetVariable(name);

			if (variable == null)
				throw new VariableMissingException(name);

			if (!DoSet(variable, value))
				throw new VariableTypeMismatchException(name, typeof(T), variable.Type);
		}

		public void Change<T>(string name, T value)
		{
			if (!HasAccess(VariableAccess.Change))
				throw new VariableAccessException(VariableAccess.Change, Access);

			var variable = GetVariable(name);

			if (variable == null)
				throw new VariableMissingException(name);

			DoChange(variable, value);
		}

		public void Add(VariableValue value)
		{
			if (!HasAccess(VariableAccess.Add))
				throw new VariableAccessException(VariableAccess.Add, Access);

			var variable = GetVariable(value.Name);

			if (variable != null)
				throw new VariableExistsException(value.Name);

			DoAdd(value);
		}

		public void AddEmpty(string name)
		{
			if (!HasAccess(VariableAccess.Add))
				throw new VariableAccessException(VariableAccess.Add, Access);

			var variable = GetVariable(name);

			if (variable != null)
				throw new VariableExistsException(name);

			DoAdd(VariableValue.CreateEmpty(name));
		}

		public void Remove(string name)
		{
			if (!HasAccess(VariableAccess.Remove))
				throw new VariableAccessException(VariableAccess.Remove, Access);

			if (!DoRemove(name))
				throw new VariableMissingException(name);
		}

		public bool TryAdd<T>(string name, T value)
		{
			if (HasAccess(VariableAccess.Add))
			{
				var variable = GetVariable(name);

				if (variable == null)
				{
					DoAdd(VariableValue.Create(name, value));
					return true;
				}
			}

			return false;
		}

		public bool TryGet<T>(string name, out T value)
		{
			if (HasAccess(VariableAccess.Get))
			{
				var variable = GetVariable(name);

				if (variable != null)
					return variable.TryGet(out value);
			}

			value = default(T);
			return false;
		}

		public bool TrySet<T>(string name, T value)
		{
			if (HasAccess(VariableAccess.Set))
			{
				var variable = GetVariable(name);

				if (variable != null)
					return DoSet(variable, value);
			}

			return false;
		}

		public bool TryChange<T>(string name, T value)
		{
			if (HasAccess(VariableAccess.Change))
			{
				var variable = GetVariable(name);

				if (variable != null)
				{
					DoChange(variable, value);
					return true;
				}
			}

			return false;
		}

		public bool TryAddEmpty(string name)
		{
			if (HasAccess(VariableAccess.Add))
			{
				var variable = GetVariable(name);

				if (variable == null)
				{
					DoAdd(VariableValue.CreateEmpty(name));
					return true;
				}
			}

			return false;
		}

		public bool TryRemove(string name)
		{
			return HasAccess(VariableAccess.Remove) && DoRemove(name);
		}

		public T GetOrDefault<T>(string name, T defaultValue)
		{
			if (HasAccess(VariableAccess.Get))
			{
				var variable = GetVariable(name);

				if (variable != null)
				{
					T value;
					if (variable.TryGet(out value))
						return value;
				}
			}

			return defaultValue;
		}

		public T GetOrAdd<T>(string name, T value)
		{
			var variable = GetVariable(name);

			if (variable == null)
			{
				if (!HasAccess(VariableAccess.Add))
					throw new VariableAccessException(VariableAccess.Add, Access);

				DoAdd(VariableValue.Create(name, value));
			}
			else
			{
				if (!HasAccess(VariableAccess.Add))
					throw new VariableAccessException(VariableAccess.Add, Access);

				if (!variable.TryGet(out value))
					throw new VariableTypeMismatchException(name, typeof(T), variable.Type);
			}

			return value;
		}

		public void SetOrAdd<T>(string name, T value)
		{
			var variable = GetVariable(name);

			if (variable == null)
			{
				if (!HasAccess(VariableAccess.Add))
					throw new VariableAccessException(VariableAccess.Add, Access);

				DoAdd(VariableValue.Create(name, value));
			}
			else
			{
				if (!HasAccess(VariableAccess.Set))
					throw new VariableAccessException(VariableAccess.Set, Access);

				if (!DoSet(variable, value))
					throw new VariableTypeMismatchException(name, typeof(T), variable.Type);
			}
		}

		public void ChangeOrAdd<T>(string name, T value)
		{
			var variable = GetVariable(name);

			if (variable == null)
			{
				if (!HasAccess(VariableAccess.Add))
					throw new VariableAccessException(VariableAccess.Add, Access);

				DoAdd(VariableValue.Create(name, value));
			}
			else
			{
				if (!HasAccess(VariableAccess.Change))
				{
					if (!HasAccess(VariableAccess.Set) || !DoSet(variable, value))
						throw new VariableAccessException(VariableAccess.Change, Access);
				}
				else
				{
					DoChange(variable, value);
				}
			}
		}

		public bool TryGetOrAdd<T>(string name, ref T value)
		{
			var variable = GetVariable(name);

			if (variable == null)
			{
				if (HasAccess(VariableAccess.Add))
				{
					DoAdd(VariableValue.Create(name, value));
					return true;
				}

				return false;
			}

			return HasAccess(VariableAccess.Get) && variable.TryGet(out value);
		}

		public bool TrySetOrAdd<T>(string name, T value)
		{
			var variable = GetVariable(name);

			if (variable == null)
			{
				if (HasAccess(VariableAccess.Add))
				{
					DoAdd(VariableValue.Create(name, value));
					return true;
				}

				return false;
			}

			return HasAccess(VariableAccess.Set) && DoSet(variable, value);
		}

		public bool Exists(int index)
		{
			return index >= 0 && index < _variables.Count;
		}

		public VariableType GetType(int index)
		{
			var variable = GetVariable(index);
			return variable != null ? variable.Type : VariableType.Empty;
		}

		public bool HasType<T>(int index)
		{
			var variable = GetVariable(index);
			return variable != null ? variable.HasType<T>() : false;
		}

		public bool HasType(int index, Type type)
		{
			var variable = GetVariable(index);
			return variable != null ? variable.HasType(type) : false;
		}

		public VariableValue GetVariable(int index)
		{
			return index >= 0 && index < _variables.Count ? _variables[index] : null;
		}

		public void Append<T>(string name, T value)
		{
			var variable = GetVariable(name);

			if (variable != null)
				throw new VariableExistsException(name);

			DoAdd(VariableValue.Create(name, value));
		}

		public T Get<T>(int index)
		{
			var variable = GetVariable(index);
			T value;

			if (variable == null)
				throw new VariableOutOfRangeException(index, _variables.Count);

			if (!variable.TryGet(out value))
				throw new VariableTypeMismatchException(variable.Name, typeof(T), variable.Type);

			return value;
		}

		public void Set<T>(int index, T value)
		{
			var variable = GetVariable(index);

			if (variable == null)
				throw new VariableOutOfRangeException(index, _variables.Count);

			if (!DoSet(variable, value))
				throw new VariableTypeMismatchException(variable.Name, typeof(T), variable.Type);
		}

		public void Change<T>(int index, T value)
		{
			var variable = GetVariable(index);

			if (variable == null)
				throw new VariableOutOfRangeException(index, _variables.Count);

			DoChange(variable, value);
		}

		public void Remove(int index)
		{
			if (index < 0 || index >= _variables.Count)
				throw new VariableOutOfRangeException(index, _variables.Count);

			DoRemove(index);
		}

		public void Chop(int index)
		{
			if (index < 0 || index > _variables.Count)
				throw new VariableOutOfRangeException(index, _variables.Count);

			for (var i = _variables.Count - 1; i >= index; i--)
				DoRemove(i);
		}

		public bool TryGet<T>(int index, out T value)
		{
			var variable = GetVariable(index);

			if (variable == null)
			{
				value = default(T);
				return false;
			}

			return variable.TryGet(out value);
		}

		public bool TrySet<T>(int index, T value)
		{
			var variable = GetVariable(index);

			if (variable == null)
				return false;

			return DoSet(variable, value);
		}

		public void Reset()
		{
			_variables.Clear();
			_variablesMap.Clear();
		}

		public void Copy(string name, VariableValue from)
		{
			var to = GetVariable(name);

			if (to != null)
			{
				DoCopy(to, from);
			}
			else
			{
				var add = from.Clone();
				add.Rename(name);
				DoAdd(add);
			}
		}

		public void Copy(VariableStore from)
		{
			foreach (var variable in from._variables)
				Copy(variable.Name, variable);
		}

		public VariableStore Clone()
		{
			var clone = new VariableStore();

			foreach (var variable in _variables)
				clone._variables.Add(variable.Clone());

			return clone;
		}

		public void Subscribe(IVariableListener listener, object owner)
		{
			_listeners.Add(new Listener { Interface = listener, Owner = owner });
		}

		public void Unsubscribe(IVariableListener listener)
		{
			for (var i = 0; i < _listeners.Count; i++)
			{
				if (_listeners[i].Interface == listener)
				{
					_listeners.RemoveAt(i);
					break;
				}
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			RebuildMap();
		}

		private struct Listener
		{
			public IVariableListener Interface;
			public object Owner;
		}

		private void DoAdd(VariableValue variable)
		{
			_variablesMap.Add(variable.Name, _variables.Count);
			_variables.Add(variable);

			for (var i = 0; i < _listeners.Count; i++)
				_listeners[i].Interface.OnVariableAdded(variable, _listeners[i].Owner);
		}

		private bool DoSet<T>(VariableValue variable, T value)
		{
			if (variable.TrySet(value))
			{
				for (var i = 0; i < _listeners.Count; i++)
					_listeners[i].Interface.OnVariableChanged(variable, _listeners[i].Owner);

				return true;
			}

			return false;
		}

		private void DoChange<T>(VariableValue variable, T value)
		{
			variable.Change(value);

			for (var i = 0; i < _listeners.Count; i++)
				_listeners[i].Interface.OnVariableChanged(variable, _listeners[i].Owner);
		}

		private void DoCopy(VariableValue variable, VariableValue from)
		{
			variable.Assign(from);

			for (var i = 0; i < _listeners.Count; i++)
				_listeners[i].Interface.OnVariableChanged(variable, _listeners[i].Owner);
		}

		private bool DoRemove(string name)
		{
			var index = GetIndex(name);

			if (index >= 0)
			{
				DoRemove(index);
				return true;
			}

			return false;
		}

		private void DoRemove(int index)
		{
			var variable = _variables[index];

			_variables.RemoveAt(index);
			VariableValue.Destroy(variable);
			RebuildMap();

			for (var i = 0; i < _listeners.Count; i++)
				_listeners[i].Interface.OnVariableRemoved(variable.Name, _listeners[i].Owner);
		}

		private void RebuildMap()
		{
			_variablesMap.Clear();

			for (var i = 0; i < _variables.Count; i++)
				_variablesMap.Add(_variables[i].Name, i);
		}
	}
}
