using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	[CustomEditor(typeof(Pathfinding))]
	class PathfindingEditor : Editor
	{
		private static readonly TextButton _regenerateNodesButton = new TextButton("Regenerate Nodes", "Regenerate pathfinding nodes on this map object");

		private Pathfinding _pathfinding;

		void OnEnable()
		{
			_pathfinding = target as Pathfinding;
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button(_regenerateNodesButton.Content))
			{
				using (new UndoScope(_pathfinding, true))
					_pathfinding.RegenerateNodes();
			}
		}
	}
}
