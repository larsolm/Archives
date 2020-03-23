using System;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace PiRhoSoft.EditorExtensions
{
	public class InstanceEditor<BaseType>
	{
		static InstanceEditor()
		{
			_types = InlineEditor.GetTypes<BaseType>();
		}

		private static InlineEditorTypes _types;

		public void Inspect(string label, UnityObject owner, ref BaseType instance)
		{
			var itemIndex = instance != null ? Array.IndexOf(_types.ItemTypes, instance.GetType()) : 0;
			var editorIndex = _editor != null ? Array.IndexOf(_types.EditorTypes, _editor.GetType()) : 0;

			using (var scope = new EditorGUI.ChangeCheckScope())
			{
				itemIndex = EditorGUILayout.Popup(label, itemIndex, _types.TypeNames);

				if (scope.changed)
				{
					Undo.RecordObject(owner, "Changed " + label);

					if (itemIndex > 0 && itemIndex < _types.Count)
						instance = (BaseType)Activator.CreateInstance(_types.ItemTypes[itemIndex]);
					else
						instance = default(BaseType);

					EditorUtility.SetDirty(owner);
				}
			}

			if (itemIndex != editorIndex)
			{
				var editorType = itemIndex > 0 && itemIndex < _types.Count ? _types.EditorTypes[itemIndex] : null;

				if (editorType != null)
					_editor = Activator.CreateInstance(editorType) as InlineEditor;
				else
					_editor = null;
			}

			if (_editor != null)
			{
				using (new EditorGUI.IndentLevelScope())
					_editor.OnInspectorGUI(instance);
			}
		}

		private InlineEditor _editor;
	}
}
