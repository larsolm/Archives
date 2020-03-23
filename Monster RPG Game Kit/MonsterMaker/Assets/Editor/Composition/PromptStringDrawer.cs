using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomPropertyDrawer(typeof(PromptString))]
	public class PromptStringDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var _message = property.FindPropertyRelative("Message");
			var _input = property.FindPropertyRelative("Input");

			var height = EditorGUI.GetPropertyHeight(_message);

			if (_input.arraySize > 0)
				height += 10 + EditorGUI.GetPropertyHeight(_input.GetArrayElementAtIndex(0)) * _input.arraySize;

			return height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var _message = property.FindPropertyRelative("Message");
			var _input = property.FindPropertyRelative("Input");

			var messageHeight = EditorGUI.GetPropertyHeight(_message);
			var messageRect = new Rect(position.x, position.y, position.width, messageHeight);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				EditorGUI.PropertyField(messageRect, _message);

				if (changes.changed)
				{
					var count = GetParameterCount(_message.stringValue);
					if (_input.arraySize != count)
						_input.arraySize = count;
				}
			}

			for (var i = 0; i < _input.arraySize; i++)
			{
				var variable = _input.GetArrayElementAtIndex(i);
				var variableHeight = EditorGUI.GetPropertyHeight(variable);
				var variableRect = new Rect(position.x, position.y + messageHeight + 10 + i * variableHeight, position.width, variableHeight);
				
				EditorGUI.PropertyField(variableRect, variable, new GUIContent("Input " + i));
			}
		}

		private static Regex _formatRegex = new Regex(@"\{([0-9])+\}", RegexOptions.Compiled);

		private int GetParameterCount(string formatString)
		{
			var count = 0;
			var matches = _formatRegex.Matches(formatString);

			for (var i = 0; i < matches.Count; i++)
			{
				var match = matches[i].Groups[1];
				var number = int.Parse(match.Value);

				if (count <= number)
					count = number + 1;
			}

			return count;
		}
	}
}
