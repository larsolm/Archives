using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Assets.Editor.Utility
{
	public static class EditorCreator<EditorAttribute, EditorType, ItemType> where EditorAttribute : Attribute where ItemType : class
	{
		static EditorCreator()
		{
		}

		public static ReadOnlyCollection<Type> Types { get; private set; }
		public static string[] TypeNames { get; private set; }

		public static EditorType CreateEditor(Type traitType)
		{
			Type editorType = null;
			if (!_editorTypes.TryGetValue(traitType, out editorType))
				editorType = typeof(EditorType);

			return (EditorType)Activator.CreateInstance(editorType);
		}

		public static ItemType CreateItem(int index)
		{
			if (index >= 0 && index < _itemTypes.Count)
				return (ItemType)Activator.CreateInstance(_itemTypes[index]);

			return null;
		}

		private static List<Type> _itemTypes = new List<Type>();
		private static Dictionary<Type, Type> _editorTypes = new Dictionary<Type, Type>();
	}
}
