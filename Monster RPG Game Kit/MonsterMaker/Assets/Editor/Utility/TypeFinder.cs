using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PiRhoSoft.MonsterMaker
{
	public class CustomEditorInfo
	{
		public int Count;
		public Type[] EditorTypes;
		public Type[] ItemTypes;
		public string[] TypeNames;
	}

	public class TypeSelectInfo
	{
		public Type[] Types;
		public string[] Names;
	}

	public class CustomControlAttribute : Attribute
	{
		public Type ObjectType;
	}

	public static class TypeFinder
	{
		public static AttributeType GetAttribute<AttributeType>(Type type) where AttributeType : Attribute
		{
			var attributes = type.GetCustomAttributes(typeof(AttributeType), false);
			return attributes != null && attributes.Length > 0 ? attributes[0] as AttributeType : null;
		}

		public static bool IsCreatableAs(Type baseType, Type type)
		{
			return baseType.IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
		}

		public static bool IsCreatableAs<BaseType>(Type type)
		{
			return IsCreatableAs(typeof(BaseType), type);
		}

		public static TypeSelectInfo GetDerivedTypes(Type baseType, string prefix)
		{
			var types = new List<Type>();
			var names = new List<string>();

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetExportedTypes())
				{
					if (IsCreatableAs(baseType, type))
					{
						types.Add(type);
						names.Add(prefix + ObjectNames.NicifyVariableName(type.Name));
					}
				}
			}

			return new TypeSelectInfo { Types = types.OrderBy(type => type.Name).ToArray(), Names = names.OrderBy(name => name).ToArray() };
		}

		public static TypeSelectInfo GetDerivedTypes<BaseType>(string prefix)
		{
			return GetDerivedTypes(typeof(BaseType), prefix);
		}

		public static List<Type> GetTypesWithAttribute<BaseType, AttributeType>() where AttributeType : Attribute
		{
			var types = new List<Type>();

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetExportedTypes())
				{
					if (IsCreatableAs<BaseType>(type))
					{
						var attribute = GetAttribute<AttributeType>(type);
						if (attribute != null)
							types.Add(type);
					}
				}
			}

			return types;
		}

		public static List<Type> GetEnumsWithAttribute<AttributeType>() where AttributeType : Attribute
		{
			var types = new List<Type>();

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetExportedTypes())
				{
					if (type.IsEnum)
					{
						var attribute = GetAttribute<AttributeType>(type);
						if (attribute != null)
							types.Add(type);
					}
				}
			}

			return types;
		}

		public static CustomEditorInfo GetCustomEditors<ItemType, EditorType, AttributeType>() where AttributeType : Attribute
		{
			var editorTypes = new List<Type>();
			var itemTypes = new List<Type>();
			var typeNames = new List<string>();

			// TODO: print warning when encountering type mismatches

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetExportedTypes())
				{
					if (IsCreatableAs<EditorType>(type))
					{
						var attribute = GetAttribute<AttributeType>(type) as CustomControlAttribute;
						if (attribute != null && IsCreatableAs<ItemType>(attribute.ObjectType))
						{
							editorTypes.Add(type);
							itemTypes.Add(attribute.ObjectType);
							typeNames.Add(attribute.ObjectType.Name);
						}
					}
				}
			}

			return new CustomEditorInfo
			{
				Count = editorTypes.Count,
				EditorTypes = editorTypes.ToArray(),
				ItemTypes = itemTypes.ToArray(),
				TypeNames = typeNames.ToArray()
			};
		}
	}
}
