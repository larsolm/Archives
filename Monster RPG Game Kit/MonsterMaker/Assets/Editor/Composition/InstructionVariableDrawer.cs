using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomPropertyDrawer(typeof(InstructionVariable))]
	public class InstructionVariableDrawer : PropertyDrawer
	{
		private static string[] _typeNames;
		private static int _locationOffset;

		public static string[] GetTypeNames()
		{
			if (_typeNames == null)
			{
				var valueTypeNames = VariableValueDrawer.GetTypeNames();
				var referenceLocationNames = VariableReferenceDrawer.GetLocationNames();

				_typeNames = new string[valueTypeNames.Length + referenceLocationNames.Length];
				_locationOffset = valueTypeNames.Length;

				for (var i = 0; i < valueTypeNames.Length; i++)
					_typeNames[i] = "Value/" + valueTypeNames[i];

				for (var i = 0; i < referenceLocationNames.Length; i++)
					_typeNames[_locationOffset + i] = "Reference/" + referenceLocationNames[i];
			}

			return _typeNames;
		}

		public int GetTypeIndex(SerializedProperty property)
		{
			var isReferenceProperty = property.FindPropertyRelative("IsReference");

			if (isReferenceProperty.boolValue)
			{
				var referenceProperty = property.FindPropertyRelative("Reference");
				var index = VariableReferenceDrawer.GetLocationIndex(referenceProperty);

				return index + _locationOffset;
			}
			else
			{
				var valueProperty = property.FindPropertyRelative("Value");
				return VariableValueDrawer.GetTypeIndex(valueProperty);
			}
		}

		public void SetTypeIndex(SerializedProperty property, int index)
		{
			var isReferenceProperty = property.FindPropertyRelative("IsReference");
			var referenceProperty = property.FindPropertyRelative("Reference");
			var valueProperty = property.FindPropertyRelative("Value");

			var wasReference = isReferenceProperty.boolValue;
			var isReference = index >= _locationOffset;

			isReferenceProperty.boolValue = isReference;

			if (wasReference && !isReference)
				VariableReferenceDrawer.ResetLocation(referenceProperty);
			else if (!wasReference && isReference)
				VariableValueDrawer.SetTypeIndex(valueProperty, 0);

			if (isReference)
				VariableReferenceDrawer.SetLocationIndex(referenceProperty, index - _locationOffset);
			else
				VariableValueDrawer.SetTypeIndex(valueProperty, index);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var typeNames = GetTypeNames();
			var nameProperty = property.FindPropertyRelative("Name");
			var isReferenceProperty = property.FindPropertyRelative("IsReference");

			var nameWidth = position.width * 0.3f;
			var typeWidth = position.width * 0.3f;
			var nameRect = new Rect(position.x, position.y, nameWidth, position.height);
			var typeRect = new Rect(nameRect.xMax + 5, position.y, typeWidth, position.height);
			var valueRect = new Rect(typeRect.xMax + 5, position.y, position.width - nameWidth - typeWidth - 10, position.height);

			var index = GetTypeIndex(property);

			EditorGUI.PropertyField(nameRect, nameProperty, GUIContent.none);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				index = EditorGUI.Popup(typeRect, index, typeNames);

				if (changes.changed)
					SetTypeIndex(property, index);
			}

			if (isReferenceProperty.boolValue)
			{
				var referenceProperty = property.FindPropertyRelative("Reference");
				VariableReferenceDrawer.DrawName(valueRect, referenceProperty);
			}
			else
			{
				var valueProperty = property.FindPropertyRelative("Value");
				VariableValueDrawer.DrawValue(valueRect, valueProperty);
			}
		}

		public static int GetTypeIndex(InstructionVariable variable)
		{
			if (variable.IsReference)
			{
				var index = VariableReferenceDrawer.GetLocationIndex(variable.Reference);
				return index + _locationOffset;
			}
			else
			{
				return VariableValueDrawer.GetTypeIndex(variable.Value);
			}
		}

		public static void SetTypeIndex(InstructionVariable variable, int index)
		{
			var wasReference = variable.IsReference;
			var isReference = index >= _locationOffset;

			variable.IsReference = isReference;

			if (wasReference && !isReference)
				VariableReferenceDrawer.ResetLocation(variable.Reference);
			else if (!wasReference && isReference)
				VariableValueDrawer.SetTypeIndex(variable.Value, 0);

			if (isReference)
				VariableReferenceDrawer.SetLocationIndex(variable.Reference, index - _locationOffset);
			else
				VariableValueDrawer.SetTypeIndex(variable.Value, index);
		}

		public static void Draw(Rect position, InstructionVariable variable, GUIContent label)
		{
			var typeNames = GetTypeNames();

			var typeWidth = position.width * 0.5f;
			var typeRect = new Rect(position.x, position.y, typeWidth, position.height);
			var valueRect = new Rect(typeRect.xMax + 5, position.y, position.width - typeWidth - 5, position.height);

			var index = GetTypeIndex(variable);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				index = EditorGUI.Popup(typeRect, index, typeNames);

				if (changes.changed)
					SetTypeIndex(variable, index);
			}

			if (variable.IsReference)
				VariableReferenceDrawer.DrawName(valueRect, variable.Reference);
			else
				VariableValueDrawer.DrawValue(valueRect, variable.Value, null, null);
		}
	}
}
