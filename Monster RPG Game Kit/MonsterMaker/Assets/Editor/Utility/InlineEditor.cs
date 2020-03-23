using System;
using System.Collections.Generic;
using PiRhoSoft.MonsterMaker;

namespace PiRhoSoft.EditorExtensions
{
	public class InlineEditorAttribute : Attribute
	{
		public InlineEditorAttribute(Type itemType)
		{
			ItemType = itemType;
		}

		public Type ItemType { get; private set; }
	}

	public class InlineEditorTypes
	{
		public int Count;
		public Type[] EditorTypes;
		public Type[] ItemTypes;
		public string[] TypeNames;
	}

	public abstract class InlineEditor
	{
		static InlineEditor()
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetExportedTypes())
				{
					if (TypeFinder.IsCreatableAs<InlineEditor>(type))
						_inlineEditorTypes.Add(type);
				}
			}
		}

		public static Type GetEditorForType(Type itemType)
		{
			foreach (var type in _inlineEditorTypes)
			{
				var attribute = TypeFinder.GetAttribute<InlineEditorAttribute>(type);
				if (attribute != null && attribute.ItemType == itemType)
					return type;
			}

			return null;
		}

		public static InlineEditorTypes GetTypes<ItemType>()
		{
			var editorTypes = new List<Type> { null };
			var itemTypes = new List<Type> { null };
			var typeNames = new List<string> { "<null>" };
			
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var itemType in assembly.GetExportedTypes())
				{
					if (TypeFinder.IsCreatableAs<ItemType>(itemType))
					{
						var editorType = GetEditorForType(itemType);
						editorTypes.Add(editorType);
						itemTypes.Add(itemType);
						typeNames.Add(itemType.Name);
					}
				}
			}

			return new InlineEditorTypes
			{
				Count = editorTypes.Count,
				EditorTypes = editorTypes.ToArray(),
				ItemTypes = itemTypes.ToArray(),
				TypeNames = typeNames.ToArray()
			};
		}

		private static List<Type> _inlineEditorTypes = new List<Type>();

		public abstract void OnInspectorGUI(object instance);
	}

	public abstract class InlineEditor<ItemType> : InlineEditor where ItemType : class
	{
		public override void OnInspectorGUI(object instance)
		{
			var item = instance as ItemType;
			if (item != null) // TODO: log if instance is not an ItemType?
				Inspect(item);
		}

		protected abstract void Inspect(ItemType instance);
	}

}
