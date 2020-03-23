using System.Linq;
using System.Reflection;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class VariableSchemaControl : PropertyControl
	{
		public VariableSchema Schema;
		public EditableList<VariableValue> Values;

		public override void Setup(SerializedProperty property, FieldInfo fieldInfo)
		{
			Schema = GetObject<VariableSchema>(property);
			Values = new EditableList<VariableValue>();

			var values = Values.Setup(Schema.Definitions, Schema.Name, EditorHelper.GetTooltip(fieldInfo), true, true, false, true, true, DrawValue, RemoveValue);
			values.onAddDropdownCallback += AddValueDropdown;
		}

		public override float GetHeight(SerializedProperty property, GUIContent label)
		{
			return Values.Height;
		}

		public override void Draw(Rect position, SerializedProperty property, GUIContent label)
		{
			Values.DrawList(position);
		}

		private class AddPopup : PopupWindowContent
		{
			public AddPopup(VariableSchema schema)
			{
				_schema = schema;
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, EditorGUIUtility.singleLineHeight * 5);
			}

			public override void OnGUI(Rect rect)
			{
				EditorGUILayout.LabelField(_label);

				var enter = GuiFields.TextEnterField("NewName", GUIContent.none, ref _newName);

				_type = (VariableType)EditorGUILayout.EnumPopup(_type);

				var create = GUILayout.Button(EditorHelper.CreateContent);

				if ((enter || create) && !string.IsNullOrEmpty(_newName))
				{
					CreateVariable(_schema, _newName, _type);
					editorWindow.Close();
				}
			}

			private GUIContent _label = new GUIContent("New Variable", "Add a new variable to the schema.");
			private VariableSchema _schema;
			private string _newName = "Name";
			private VariableType _type = VariableType.Empty;

			private void CreateVariable(VariableSchema schema, string name, VariableType type)
			{
				var names = schema.Definitions.Select(value => value.Name).ToArray();
				var uniqueName = ObjectNames.GetUniqueName(names, name);

				schema.AddDefinition(uniqueName, type);
			}
		}

		private void DrawValue(Rect rect, int index)
		{
			var definition = Schema.Definitions[index];
			var nameRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
			var typeRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

			EditorGUI.LabelField(nameRect, definition.Name);
			EditorGUI.LabelField(typeRect, ObjectNames.NicifyVariableName(definition.Type.ToString()));
		}

		private void AddValueDropdown(Rect rect, ReorderableList list)
		{
			rect.y += EditorGUIUtility.singleLineHeight;
			PopupWindow.Show(rect, new AddPopup(Schema));
		}

		private void RemoveValue(int index)
		{
			Schema.Definitions.RemoveAt(index);
		}
	}

	[CustomPropertyDrawer(typeof(VariableSchema))]
	public class VariableSchemaDrawer : ControlDrawer<VariableSchemaControl>
	{
	}
}
