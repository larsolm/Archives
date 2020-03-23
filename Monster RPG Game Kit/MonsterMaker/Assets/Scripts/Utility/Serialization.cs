using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public static class Serialization
	{
		[Serializable]
		public class JsonData
		{
			public string Type;
			public string Data;
		}

		public static JsonData WriteToJson<T>(T item)
		{
			if (item != null)
			{
				var type = item.GetType().AssemblyQualifiedName;
				var data = JsonUtility.ToJson(item);

				return new JsonData { Type = type, Data = data };
			}

			return null;
		}

		public static List<JsonData> WriteToJson<T>(List<T> items)
		{
			if (items != null)
			{
				var infos = new List<JsonData>();

				for (var i = 0; i < items.Count; i++)
				{
					var info = WriteToJson(items[i]);
					infos.Add(info);
				}

				return infos;
			}

			return null;
		}

		public static T CreateFromJson<T>(JsonData info) where T : class
		{
			if (info != null)
			{
				try
				{
					var type = Type.GetType(info.Type);
					var item = Activator.CreateInstance(type) as T;

					JsonUtility.FromJsonOverwrite(info.Data, item);

					return item;
				}
				catch
				{
				}
			}

			return null;
		}

		public static List<T> CreateFromJson<T>(List<JsonData> infos) where T : class
		{
			if (infos != null)
			{
				var items = new List<T>();

				for (var i = 0; i < infos.Count; i++)
				{
					var item = CreateFromJson<T>(infos[i]);
					items.Add(item);
				}

				return items;
			}

			return null;
		}
	}
}
