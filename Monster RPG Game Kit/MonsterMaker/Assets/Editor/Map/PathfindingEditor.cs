using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(Pathfinding))]
	public class PathfindingEditor : Editor
	{
		private GUIStyle _textStyle = new GUIStyle();

		private void OnEnable()
		{
			_textStyle.normal.textColor = Color.black;
			_textStyle.alignment = TextAnchor.MiddleCenter;
		}

		public override void OnInspectorGUI()
		{
			var pathfinding = target as Pathfinding;

			if (GUILayout.Button("Regenerate Nodes"))
			{
				using (new UndoScope(pathfinding))
					pathfinding.RegenerateNodes();
			}
		}
	}
}
