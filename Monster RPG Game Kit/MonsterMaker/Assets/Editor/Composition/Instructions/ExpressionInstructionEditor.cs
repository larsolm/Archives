using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(ExpressionInstruction))]
	public class ExpressionInstructionEditor : Editor
	{
		private EditableList<string> _expressions = new EditableList<string>();

		private void OnEnable()
		{
			var expressions = target as ExpressionInstruction;

			_expressions.Setup(expressions.Statements, "Statements", null, false, true, false, true, true, DrawStatement);
		}

		private void DrawStatement(Rect rect, int index)
		{
			var expressions = target as ExpressionInstruction;
			
			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				var statement = EditorGUI.DelayedTextField(rect, expressions.Statements[index].Statement);

				if (changes.changed)
					expressions.Statements[index].Statement = statement;
			}
		}

		public override void OnInspectorGUI()
		{
			var expressions = target as ExpressionInstruction;

			InstructionBreadcrumbsDrawer.Draw();

			using (new UndoScope(expressions))
				_expressions.DrawList();
		}
	}
}
