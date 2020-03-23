using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomPropertyDrawer(typeof(InstructionBreadcrumbsAttribute))]
	public class InstructionBreadcrumbsDrawer : PropertyDrawer
	{
		private static float _height { get { return EditorGUIUtility.singleLineHeight + 10; } }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return _height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Draw(position);
		}

		public static void Draw(Rect position)
		{
			var back = new Rect(position.x, position.y + EditorGUIUtility.standardVerticalSpacing, 80, EditorGUIUtility.singleLineHeight);
			var root = new Rect(position.center.x - 40, position.y + EditorGUIUtility.standardVerticalSpacing, 80, EditorGUIUtility.singleLineHeight);
			var forward = new Rect(position.xMax - 80 - 5, position.y + EditorGUIUtility.standardVerticalSpacing, 80, EditorGUIUtility.singleLineHeight);
			
			if (InstructionBreadcrumbs.HasPreviousEntry())
			{
				if (GUI.Button(back, EditorHelper.BackContent))
					InstructionBreadcrumbs.Back();
			}

			if (InstructionBreadcrumbs.HasRoot())
			{
				if (GUI.Button(root, EditorHelper.RootContent))
					InstructionBreadcrumbs.Root();
			}

			if (InstructionBreadcrumbs.HasNextEntry())
			{
				if (GUI.Button(forward, EditorHelper.ForwardContent))
					InstructionBreadcrumbs.Forward();
			}
		}

		public static void Draw()
		{
			GUILayout.Space(_height);
			var position = GUILayoutUtility.GetLastRect();
			var rect = new Rect(position.x, position.y, EditorGUIUtility.currentViewWidth - position.x, _height);

			Draw(rect);
		}
	}
}
