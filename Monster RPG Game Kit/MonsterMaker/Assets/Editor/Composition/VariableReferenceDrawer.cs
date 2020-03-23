using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomPropertyDrawer(typeof(VariableReference))]
	public class VariableReferenceDrawer : PropertyDrawer
	{
		private static string[] _locationNames;
		private static List<Type> _sourceTypes;
		private static Dictionary<string, int> _sourceTypeOffsets;
		private static int _noneIndex;

		public static string[] GetLocationNames()
		{
			if (_locationNames == null)
			{
				_sourceTypes = TypeFinder.GetEnumsWithAttribute<VariableSourceAttribute>();
				_sourceTypeOffsets = new Dictionary<string, int>();

				var names = new List<string>();
				names.Add("Owner");
				names.Add("Parent");
				names.Add("");
				names.Add("Scene");
				names.Add("Context");
				names.Add("");

				var offset = 6;

				foreach (var sourceType in _sourceTypes)
				{
					var attribute = (sourceType.GetCustomAttributes(typeof(VariableSourceAttribute), false)[0] as VariableSourceAttribute);
					var enumNames = Enum.GetNames(sourceType);

					if (attribute.Parameters1 != null)
					{
						if (attribute.Parameters2 != null)
						{
							foreach (var parameter1 in attribute.Parameters1)
							{
								foreach (var parameter2 in attribute.Parameters2)
								{
									var baseName = string.Format(attribute.Name, parameter1, parameter2);

									foreach (var name in enumNames)
										names.Add(string.Format("{0}/{1}", baseName, ObjectNames.NicifyVariableName(name)));

									_sourceTypeOffsets.Add(baseName, offset);
									offset += enumNames.Length;
								}
							}
						}
						else
						{
							foreach (var parameter in attribute.Parameters1)
							{
								var baseName = string.Format(attribute.Name, parameter);

								foreach (var name in enumNames)
									names.Add(string.Format("{0}/{1}", baseName, ObjectNames.NicifyVariableName(name)));

								_sourceTypeOffsets.Add(baseName, offset);
								offset += enumNames.Length;
							}
						}
					}
					else
					{
						foreach (var name in enumNames)
							names.Add(string.Format("{0}/{1}", attribute.Name, ObjectNames.NicifyVariableName(name)));

						_sourceTypeOffsets.Add(attribute.Name, offset);
						offset += enumNames.Length;
					}
				}

				names.Add("");
				names.Add("None");

				_noneIndex = names.Count - 1;
				_locationNames = names.ToArray();
			}

			return _locationNames;
		}

		public static void ResetLocation(VariableReference variable)
		{
			variable.Location = VariableLocation.None;
			variable.CustomSource = null;
			variable.CustomIndex = 0;
			variable.Name = "";
		}

		public static int GetLocationIndex(VariableReference variable)
		{
			switch (variable.Location)
			{
				case VariableLocation.Owner: return 0;
				case VariableLocation.Parent: return 1;
				case VariableLocation.Scene: return 3;
				case VariableLocation.Context: return 4;
				case VariableLocation.Custom:
				{
					int offset;
					if (_sourceTypeOffsets.TryGetValue(variable.CustomSource, out offset))
						return offset + variable.CustomIndex;

					break;
				}
			}

			return _noneIndex;
		}

		public static void SetLocationIndex(VariableReference variable, int index)
		{
			GetLocationNames();

			variable.CustomSource = null;
			variable.CustomIndex = 0;

			switch (index)
			{
				case 0: variable.Location = VariableLocation.Owner; return;
				case 1: variable.Location = VariableLocation.Parent; return;
				case 3: variable.Location = VariableLocation.Scene; return;
				case 4: variable.Location = VariableLocation.Context; return;
			}

			if (index != _noneIndex)
			{
				var closest = 0;
				var sourceName = "";

				foreach (var source in _sourceTypeOffsets)
				{
					if (index > source.Value && source.Value > closest)
					{
						closest = source.Value;
						sourceName = source.Key;
					}
				}

				if (closest > 0)
				{
					variable.Location = VariableLocation.Custom;
					variable.CustomSource = sourceName;
					variable.CustomIndex = index - closest;
					return;
				}
			}

			variable.Location = VariableLocation.None;
		}

		public static void ResetLocation(SerializedProperty property)
		{
			property.FindPropertyRelative("Location").enumValueIndex = (int)VariableLocation.None;
			property.FindPropertyRelative("CustomSource").stringValue = null;
			property.FindPropertyRelative("CustomIndex").intValue = 0;
			property.FindPropertyRelative("Name").stringValue = "";
		}

		public static int GetLocationIndex(SerializedProperty property)
		{
			GetLocationNames();

			var locationProperty = property.FindPropertyRelative("Location");
			var customSourceProperty = property.FindPropertyRelative("CustomSource");
			var customIndexProperty = property.FindPropertyRelative("CustomIndex");

			var location = (VariableLocation)locationProperty.enumValueIndex;

			switch (location)
			{
				case VariableLocation.Owner: return 0;
				case VariableLocation.Parent: return 1;
				case VariableLocation.Scene: return 3;
				case VariableLocation.Context: return 4;
				case VariableLocation.Custom:
				{
					var customSource = customSourceProperty.stringValue;
					var customIndex = customIndexProperty.intValue;

					int offset;
					if (_sourceTypeOffsets.TryGetValue(customSource, out offset))
						return offset + customIndex;

					break;
				}
			}

			return _noneIndex;
		}

		public static void SetLocationIndex(SerializedProperty property, int index)
		{
			GetLocationNames();

			var locationProperty = property.FindPropertyRelative("Location");
			var customSourceProperty = property.FindPropertyRelative("CustomSource");
			var customIndexProperty = property.FindPropertyRelative("CustomIndex");

			customSourceProperty.stringValue = null;
			customIndexProperty.intValue = 0;

			switch (index)
			{
				case 0: locationProperty.enumValueIndex = (int)VariableLocation.Owner; return;
				case 1: locationProperty.enumValueIndex = (int)VariableLocation.Parent; return;
				case 3: locationProperty.enumValueIndex = (int)VariableLocation.Scene; return;
				case 4: locationProperty.enumValueIndex = (int)VariableLocation.Context; return;
			}

			if (index != _noneIndex)
			{
				var closest = 0;
				var sourceName = "";

				foreach (var source in _sourceTypeOffsets)
				{
					if (index > source.Value && source.Value > closest)
					{
						closest = source.Value;
						sourceName = source.Key;
					}
				}

				if (closest > 0)
				{
					locationProperty.enumValueIndex = (int)VariableLocation.Custom;
					customSourceProperty.stringValue = sourceName;
					customIndexProperty.intValue = index - closest;
					return;
				}
			}

			locationProperty.enumValueIndex = (int)VariableLocation.None;
		}

		public static void DrawName(Rect rect, SerializedProperty property)
		{
			var nameProperty = property.FindPropertyRelative("Name");
			var locationProperty = property.FindPropertyRelative("Location");
			var disable = locationProperty.enumValueIndex == (int)VariableLocation.Owner || locationProperty.enumValueIndex == (int)VariableLocation.Parent || locationProperty.enumValueIndex == (int)VariableLocation.None;

			if (disable) GUI.enabled = false;
			EditorGUI.PropertyField(rect, nameProperty, GUIContent.none);
			if (disable) GUI.enabled = true;
		}

		public static void DrawName(Rect rect, VariableReference variable)
		{
			var disable = variable.Location == VariableLocation.Owner || variable.Location == VariableLocation.Parent || variable.Location == VariableLocation.None;

			if (disable) GUI.enabled = false;
			variable.Name = EditorGUI.TextField(rect, variable.Name);
			if (disable) GUI.enabled = true;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var locations = GetLocationNames();
			var index = GetLocationIndex(property);

			label.tooltip = EditorHelper.GetTooltip(fieldInfo);
			position = EditorGUI.PrefixLabel(position, label);

			var width = position.width * 0.5f;
			var space = 5.0f;
			var popupRect = new Rect(position.x, position.y, width, position.height);
			var textRect = new Rect(popupRect.xMax + space, position.y, width - space, position.height);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				index = EditorGUI.Popup(popupRect, index, locations);

				if (changes.changed)
					SetLocationIndex(property, index);
			}

			DrawName(textRect, property);
		}
	}
}
