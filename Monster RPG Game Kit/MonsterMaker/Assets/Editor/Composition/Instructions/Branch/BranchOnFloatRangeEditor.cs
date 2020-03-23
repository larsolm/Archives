using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(BranchOnFloatRange))]
	public class BranchOnFloatRangeEditor : BranchOnRangeEditor<float>
	{
		protected override bool DrawNewRangeField(ref float range)
		{
			return GuiFields.FloatEnterField("FloatRange", GUIContent.none, ref range);
		}

		protected override float DrawRange(Rect rect, float range, BranchOnRange<float>.Range min, BranchOnRange<float>.Range max)
		{
			//return EditorGUI.Slider(rect, range, min == null ? float.MinValue + 0.01f : min.Value + 0.01f, max == null ? float.MaxValue - 0.01f : max.Value - 0.01f);
			return EditorGUI.DelayedFloatField(rect, range);
		}
	}
}
