using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(BranchOnIntRange))]
	public class BranchOnIntRangeEditor : BranchOnRangeEditor<int>
	{
		protected override bool DrawNewRangeField(ref int range)
		{
			return GuiFields.IntEnterField("IntRange", GUIContent.none, ref range);
		}

		protected override int DrawRange(Rect rect, int range, BranchOnRange<int>.Range min, BranchOnRange<int>.Range max)
		{
			return EditorGUI.IntSlider(rect, range, min == null ? int.MinValue + 1 : min.Value + 1, max == null ? int.MaxValue - 1 : max.Value - 1);
			//return EditorGUI.DelayedIntField(rect, range);
		}
	}
}
