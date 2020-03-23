using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public enum VariableType
	{
		Empty,
		Boolean,
		Integer,
		Number,
		String,
		Asset,
		GameObject,
		Other
	}

	[Serializable]
	public class VariableValue : IPoolable
	{
		public const int PoolSize = 1000;
		public const int PoolGrowth = 100;
		public static Pool<VariableValue> Pool = new Pool<VariableValue>(PoolSize, PoolGrowth);

		[SerializeField] private string _name;
		[SerializeField] private VariableType _type = VariableType.Empty;
		[SerializeField] private bool _boolean;
		[SerializeField] private int _integer;
		[SerializeField] private float _number;
		[SerializeField] private string _string;
		[SerializeField] private ScriptableObject _asset;
		[SerializeField] private GameObject _gameObject;
		private object _other;

		public static VariableValue Create<T>(string name, T value)
		{
			var variable = Pool.Reserve();
			variable.Rename(name);
			variable.Change(value);
			return variable;
		}

		public static VariableValue Create(string name, VariableType type)
		{
			var variable = Pool.Reserve();
			variable.Rename(name);
			variable.ChangeType(type);
			return variable;
		}

		public static VariableValue CreateEmpty(string name)
		{
			var variable = Pool.Reserve();
			variable.Rename(name);
			return variable;
		}

		public static void Destroy(VariableValue value)
		{
			Pool.Release(value);
		}

		public string Name { get { return _name; } }
		public VariableType Type { get { return _type; } }

		public override string ToString()
		{
			switch (Type)
			{
				case VariableType.Empty: return "(empty)";
				case VariableType.Boolean: return _boolean.ToString();
				case VariableType.Integer: return _integer.ToString();
				case VariableType.Number: return _number.ToString();
				case VariableType.String: return _string;
				case VariableType.Asset: return _asset.name;
				case VariableType.GameObject: return _gameObject.name;
				case VariableType.Other: return _other.ToString();
			}

			return "(unknown)";
		}

		public bool HasType<T>()
		{
			switch (Type)
			{
				case VariableType.Empty: return false;
				case VariableType.Boolean: return typeof(T) == typeof(bool);
				case VariableType.Integer: return typeof(T) == typeof(int);
				case VariableType.Number: return typeof(T) == typeof(float);
				case VariableType.String: return typeof(T) == typeof(string);
				case VariableType.Asset: return _asset is T;
				case VariableType.GameObject: return typeof(T) == typeof(GameObject);
				case VariableType.Other: return _other is T;
			}

			return false;
		}

		public bool HasType(Type type)
		{
			switch (Type)
			{
				case VariableType.Empty: return false;
				case VariableType.Boolean: return type == typeof(bool);
				case VariableType.Integer: return type == typeof(int);
				case VariableType.Number: return type == typeof(float);
				case VariableType.String: return type == typeof(string);
				case VariableType.Asset: return type.IsAssignableFrom(_asset.GetType());
				case VariableType.GameObject: return type == typeof(GameObject);
				case VariableType.Other: return type.IsAssignableFrom(_other.GetType());
			}

			return false;
		}

		public void Rename(string name)
		{
			_name = name;
		}

		public void ChangeType(VariableType type)
		{
			Reset();
			_type = type;
		}

		public void Reset()
		{
			_type = VariableType.Empty;
			_boolean = false;
			_integer = 0;
			_number = 0.0f;
			_string = string.Empty;
			_asset = null;
			_gameObject = null;
			_other = null;
		}

		public void Assign(VariableValue from)
		{
			_type = from.Type;
			_boolean = from._boolean;
			_integer = from._integer;
			_number = from._number;
			_string = from._string;
			_asset = from._asset;
			_gameObject = from._gameObject;
			_other = from._other;
		}

		public VariableValue Clone()
		{
			var copy = Pool.Reserve();
			copy.Rename(_name);
			copy.Assign(this);
			return copy;
		}

		public bool TryGet<T>(out T value)
		{
			var converter = GetConverter() as Converter<T>;
			if (converter != null)
			{
				value = converter.Get(this);
				return true;
			}
			else if (Type == VariableType.Other && _other is T)
			{
				value = (T)_other;
				return true;
			}

			value = default(T);
			return false;
		}

		public bool TrySet<T>(T value)
		{
			var converter = GetConverter() as Converter<T>;
			if (converter != null)
			{
				converter.Set(this, value);
				return true;
			}
			else if (Type == VariableType.Empty)
			{
				Change(value);
				return true;
			}
			else if (Type == VariableType.Other)
			{
				_other = value;
				return true;
			}

			return false;
		}

		public void Change<T>(T value)
		{
			var converter = GetConverter<T>();
			Reset();

			if (converter != null)
			{
				converter.Change(this, value);
			}
			else
			{
				_type = VariableType.Other;
				_other = value;
			}
		}

		public bool GetBoolean()
		{
			return _boolean;
		}

		public int GetInteger()
		{
			 return _integer;
		}

		public float GetNumber()
		{
			return _number;
		}

		public string GetString()
		{
			return _string;
		}

		public ScriptableObject GetAsset()
		{
			return _asset;
		}

		public GameObject GetGameObject()
		{
			return _gameObject;
		}

		public object GetOther()
		{
			return _other;
		}

		public bool TryGetBoolean(out bool value)
		{
			value = _boolean;
			return Type == VariableType.Boolean;
		}

		public bool TryGetInteger(out int value)
		{
			value = _integer;
			return Type == VariableType.Integer;
		}

		public bool TryGetNumber(out float value)
		{
			value = _number;
			return Type == VariableType.Number;
		}

		public bool TryGetString(out string value)
		{
			value = _string;
			return Type == VariableType.String;
		}

		public bool TryGetAsset(out ScriptableObject value)
		{
			value = _asset;
			return Type == VariableType.Asset;
		}

		public bool TryGetGameObject(out GameObject value)
		{
			value = _gameObject;
			return Type == VariableType.GameObject;
		}

		public bool TryGetOther(out object value)
		{
			value = _other;
			return Type == VariableType.Other;
		}

		public bool TrySetBoolean(bool value)
		{
			if (Type == VariableType.Boolean)
			{
				_boolean = value;
				return true;
			}

			return false;
		}

		public bool TrySetInteger(int value)
		{
			if (Type == VariableType.Integer)
			{
				_integer = value;
				return true;
			}

			return false;
		}

		public bool TrySetNumber(float value)
		{
			if (Type == VariableType.Number)
			{
				_number = value;
				return true;
			}

			return false;
		}

		public bool TrySetString(string value)
		{
			if (Type == VariableType.String)
			{
				_string = value;
				return true;
			}

			return false;
		}

		public bool TrySetAsset(ScriptableObject value)
		{
			if (Type == VariableType.Asset)
			{
				_asset = value;
				return true;
			}

			return false;
		}

		public bool TrySetGameObject(GameObject value)
		{
			if (Type == VariableType.GameObject)
			{
				_gameObject = value;
				return true;
			}

			return false;
		}

		public bool TrySetOther(object value)
		{
			if (Type == VariableType.Other)
			{
				_other = value;
				return true;
			}

			return false;
		}

		public void ChangeToBoolean(bool value)
		{
			Reset();
			_type = VariableType.Boolean;
			_boolean = value;
		}

		public void ChangeToInteger(int value)
		{
			Reset();
			_type = VariableType.Integer;
			_integer = value;
		}

		public void ChangeToNumber(float value)
		{
			Reset();
			_type = VariableType.Number;
			_number = value;
		}

		public void ChangeToString(string value)
		{
			Reset();
			_type = VariableType.String;
			_string = value;
		}

		public void ChangeToAsset(ScriptableObject value)
		{
			Reset();
			_type = VariableType.Asset;
			_asset = value;
		}

		public void ChangeToGameObject(GameObject value)
		{
			Reset();
			_type = VariableType.GameObject;
			_gameObject = value;
		}

		public void ChangeToOther(object value)
		{
			Reset();
			_type = VariableType.Other;
			_other = value;
		}

		// TODO: determine if this nonsense is actually better than a box/unbox for value type conversions
		// TODO: pattern matching in c# 7.0 likely removes the need for this
		private abstract class Converter
		{
			public VariableType Type;
		}

		private abstract class Converter<T> : Converter
		{
			public T Get(VariableValue variable)
			{
				return Get_(variable);
			}

			public void Set(VariableValue variable, T value)
			{
				Set_(variable, value);
			}

			public void Change(VariableValue variable, T value)
			{
				variable._type = Type;
				Set_(variable, value);
			}

			protected abstract T Get_(VariableValue variable);
			protected abstract void Set_(VariableValue variable, T value);
		}

		private class BooleanConverter : Converter<bool>
		{
			protected override bool Get_(VariableValue variable) { return variable._boolean; }
			protected override void Set_(VariableValue variable, bool value) { variable._boolean = value; }
		}

		private class IntegerConverter : Converter<int>
		{
			protected override int Get_(VariableValue variable) { return variable._integer; }
			protected override void Set_(VariableValue variable, int value) { variable._integer = value; }
		}

		private class NumberConverter : Converter<float>
		{
			protected override float Get_(VariableValue variable) { return variable._number; }
			protected override void Set_(VariableValue variable, float value) { variable._number = value; }
		}

		private class StringConverter : Converter<string>
		{
			protected override string Get_(VariableValue variable) { return variable._string; }
			protected override void Set_(VariableValue variable, string value) { variable._string = value; }
		}

		private class AssetConverter : Converter<ScriptableObject>
		{
			protected override ScriptableObject Get_(VariableValue variable) { return variable._asset; }
			protected override void Set_(VariableValue variable, ScriptableObject value) { variable._asset = value; }
		}

		private class GameObjectConverter : Converter<GameObject>
		{
			protected override GameObject Get_(VariableValue variable) { return variable._gameObject; }
			protected override void Set_(VariableValue variable, GameObject value) { variable._gameObject = value; }
		}

		private class OtherConverter : Converter<object>
		{
			protected override object Get_(VariableValue variable) { return variable._other; }
			protected override void Set_(VariableValue variable, object value) { variable._other = value; }
		}

		private static BooleanConverter _booleanConverter = new BooleanConverter { Type = VariableType.Boolean };
		private static IntegerConverter _integerConverter = new IntegerConverter { Type = VariableType.Integer };
		private static NumberConverter _numberConverter = new NumberConverter { Type = VariableType.Number };
		private static StringConverter _stringConverter = new StringConverter { Type = VariableType.String };
		private static AssetConverter _assetConverter = new AssetConverter { Type = VariableType.Asset };
		private static GameObjectConverter _gameObjectConverter = new GameObjectConverter { Type = VariableType.GameObject };
		private static OtherConverter _otherConverter = new OtherConverter { Type = VariableType.Other };

		private Converter GetConverter()
		{
			switch (Type)
			{
				case VariableType.Empty: return null;
				case VariableType.Boolean: return _booleanConverter;
				case VariableType.Integer: return _integerConverter;
				case VariableType.Number: return _numberConverter;
				case VariableType.String: return _stringConverter;
				case VariableType.Asset: return _assetConverter;
				case VariableType.GameObject: return _gameObjectConverter;
				case VariableType.Other: return _otherConverter;
			}

			return null;
		}
		
		private Converter<T> GetConverter<T>()
		{
			if (typeof(T) == typeof(bool)) return _booleanConverter as Converter<T>;
			else if (typeof(T) == typeof(int)) return _integerConverter as Converter<T>;
			else if (typeof(T) == typeof(float)) return _numberConverter as Converter<T>;
			else if (typeof(T) == typeof(string)) return _stringConverter as Converter<T>;
			else if (typeof(ScriptableObject).IsAssignableFrom(typeof(T))) return _assetConverter as Converter<T>;
			else if (typeof(T) == typeof(GameObject)) return _gameObjectConverter as Converter<T>;
			else return _otherConverter as Converter<T>;
		}
	}
}
