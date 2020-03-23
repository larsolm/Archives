using System;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomPropertyDrawer(typeof(VariableValue))]
	public class VariableValueDrawer : PropertyDrawer
	{
		private static string[] _typeNames;

		public static string[] GetTypeNames()
		{
			if (_typeNames == null)
			{
				var names = Enum.GetNames(typeof(VariableType));

				_typeNames = new string[names.Length];

				for (var i = 0; i < names.Length; i++)
					_typeNames[i] = ObjectNames.NicifyVariableName(names[i]);
			}

			return _typeNames;
		}

		public static void Draw(Rect rect, SerializedProperty property, GUIContent label, bool readOnly)
		{
			var nameWidth = rect.width * 0.3f;
			var typeWidth = rect.width * 0.3f;
			var valueWidth = rect.width - nameWidth - typeWidth - 10;

			var nameRect = new Rect(rect.x, rect.y, nameWidth, rect.height);
			var typeRect = new Rect(nameRect.xMax + 5, rect.y, typeWidth, rect.height);
			var valueRect = new Rect(typeRect.xMax + 5, rect.y, valueWidth, rect.height);

			DrawName(nameRect, property, readOnly);
			DrawType(typeRect, property);
			DrawValue(valueRect, property);
		}

		public static void DrawName(Rect rect, SerializedProperty property, bool readOnly)
		{
			var nameProperty = property.FindPropertyRelative("_name");

			if (readOnly) 
				EditorGUI.LabelField(rect, nameProperty.stringValue);
			else
				EditorGUI.PropertyField(rect, nameProperty, GUIContent.none);
		}

		public static int GetTypeIndex(SerializedProperty property)
		{
			return property.FindPropertyRelative("_type").enumValueIndex;
		}

		public static void SetTypeIndex(SerializedProperty property, int index)
		{
			var typeProperty = property.FindPropertyRelative("_type");

			if (typeProperty.enumValueIndex != index)
			{
				ResetType(property);
				typeProperty.enumValueIndex = index;
			}
		}

		public static int GetTypeIndex(VariableValue variable)
		{
			return (int)variable.Type;
		}

		public static void SetTypeIndex(VariableValue variable, int index)
		{
			variable.ChangeType((VariableType)index);
		}

		public static void DrawType(Rect rect, SerializedProperty property)
		{
			var typeNames = GetTypeNames();
			var index = GetTypeIndex(property);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				index = EditorGUI.Popup(rect, index, typeNames);

				if (changes.changed)
					SetTypeIndex(property, index);
			}
		}

		public static void DrawValue(Rect rect, SerializedProperty property)
		{
			var typeProperty = property.FindPropertyRelative("_type");
			var type = (VariableType)typeProperty.enumValueIndex;

			switch (type)
			{
				case VariableType.Empty: break;
				case VariableType.Boolean: EditorGUI.PropertyField(rect, property.FindPropertyRelative("_boolean"), GUIContent.none); break;
				case VariableType.Integer: EditorGUI.PropertyField(rect, property.FindPropertyRelative("_integer"), GUIContent.none); break;
				case VariableType.Number: EditorGUI.PropertyField(rect, property.FindPropertyRelative("_number"), GUIContent.none); break;
				case VariableType.String: EditorGUI.PropertyField(rect, property.FindPropertyRelative("_string"), GUIContent.none); break;
				case VariableType.Asset: EditorGUI.PropertyField(rect, property.FindPropertyRelative("_asset"), GUIContent.none); break;
				case VariableType.GameObject: EditorGUI.PropertyField(rect, property.FindPropertyRelative("_gameObject"), GUIContent.none); break;
				case VariableType.Other: break;
			}
		}

		public static void DrawValue(Rect rect, VariableValue variable, Type assetType, Type assetParentType)
		{
			switch (variable.Type)
			{
				case VariableType.Empty: break;
				case VariableType.Boolean: variable.TrySetBoolean(EditorGUI.Toggle(rect, variable.GetBoolean())); break;
				case VariableType.Integer: variable.TrySetInteger(EditorGUI.IntField(rect, variable.GetInteger())); break;
				case VariableType.Number: variable.TrySetNumber(EditorGUI.FloatField(rect, variable.GetNumber())); break;
				case VariableType.String: variable.TrySetString(EditorGUI.TextField(rect, variable.GetString())); break;
				case VariableType.Asset:
				{
					var asset = variable.GetAsset();

					if (assetParentType != null)
						GuiFields.SubAssetPopup(rect, GUIContent.none, assetType, assetParentType, ref asset);
					else if (assetType != null)
						GuiFields.MainAssetPopup(rect, GUIContent.none, assetType, ref asset);
					else
						asset = (ScriptableObject)EditorGUI.ObjectField(rect, asset, typeof(ScriptableObject), false);

					variable.TrySetAsset(asset);
					break;
				}
				case VariableType.GameObject: variable.TrySetGameObject((GameObject)EditorGUI.ObjectField(rect, variable.GetGameObject(), typeof(GameObject), true)); break;
				case VariableType.Other: break;
			}
		}

		public static void ResetType(SerializedProperty property)
		{
			property.FindPropertyRelative("_boolean").boolValue = false;
			property.FindPropertyRelative("_integer").intValue = 0;
			property.FindPropertyRelative("_number").floatValue = 0.0f;
			property.FindPropertyRelative("_string").stringValue = string.Empty;
			property.FindPropertyRelative("_asset").objectReferenceValue = null;
			property.FindPropertyRelative("_gameObject").objectReferenceValue = null;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Draw(position, property, GUIContent.none, false);
		}
	}
}
