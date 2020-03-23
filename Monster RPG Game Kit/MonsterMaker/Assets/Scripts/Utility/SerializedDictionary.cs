using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class SerializedDictionary<KeyType, ValueType> : Dictionary<KeyType, ValueType>, ISerializationCallbackReceiver
	{
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			_keys.Clear();
			_values.Clear();

			foreach (var entry in this)
			{
				_keys.Add(entry.Key);
				_values.Add(entry.Value);
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			Clear();

			for (var i = 0; i < Math.Min(_keys.Count, _values.Count); i++)
				Add(_keys[i], _values[i]);
		}

		[SerializeField] private List<KeyType> _keys = new List<KeyType>();
		[SerializeField] private List<ValueType> _values = new List<ValueType>();
	}
}
