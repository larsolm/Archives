using UnityEngine;
using UnityEditor;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(Object), true)]
	public class DefaultScriptEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.Space();

			serializedObject.Update();

			var iterator = serializedObject.GetIterator();
			if (iterator.NextVisible(true))
			{
				while (iterator.NextVisible(false))
					EditorGUILayout.PropertyField(iterator, true);
			}

			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.Space();
		}
	}
}
