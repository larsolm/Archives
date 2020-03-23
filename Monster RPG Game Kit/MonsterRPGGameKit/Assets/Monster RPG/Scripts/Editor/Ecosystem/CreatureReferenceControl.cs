using System.Reflection;
using PiRhoSoft.CompositionEditor;
using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	public class CreatureReferenceControl : ObjectControl<CreatureReference>
	{
		private static readonly IconButton _editCreatureButton = new IconButton(IconButton.Edit, "Edit this creature");
		private static readonly IconButton _editSpeciesButton = new IconButton(IconButton.Edit, "Edit this species");

		public CreatureReference Creature { get; private set; }

		private InstructionCallerControl _instructionCaller;

		public override void Setup(CreatureReference target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			Creature = target;

			_instructionCaller = new InstructionCallerControl();
			_instructionCaller.Setup(target.Generator, null, null, null);
		}

		public override float GetHeight(GUIContent label)
		{
			var height = 0.0f;

			if (Creature.Species != null)
				height += _instructionCaller.GetHeight(null);

			if (Creature.Species != null && Creature.Creature != null)
				height += RectHelper.VerticalSpace;

			if (Creature.Creature != null)
				height += EditorGUIUtility.singleLineHeight;

			return height;
		}

		public override void Draw(Rect position, GUIContent label)
		{
			if (Creature.Species != null)
			{
				var speciesHeight = _instructionCaller.GetHeight(null);
				var speciesRect = RectHelper.TakeHeight(ref position, speciesHeight);

				RectHelper.TakeVerticalSpace(ref position);

				var labelRect = RectHelper.TakeWidth(ref speciesRect, speciesRect.width * 0.25f);
				var editRect = RectHelper.AdjustHeight(RectHelper.TakeTrailingIcon(ref speciesRect), EditorGUIUtility.singleLineHeight, RectVerticalAlignment.Top);

				EditorGUI.LabelField(labelRect, Creature.Species.Name);
				_instructionCaller.Draw(speciesRect, GUIContent.none);

				if (GUI.Button(editRect, _editSpeciesButton.Content, GUIStyle.none))
					Selection.activeObject = Creature.Species;
			}

			if (Creature.Creature != null)
			{
				var editRect = RectHelper.TakeTrailingIcon(ref position);
				EditorGUI.LabelField(position, Creature.Creature.Name);

				if (GUI.Button(editRect, _editCreatureButton.Content, GUIStyle.none))
					Selection.activeObject = Creature.Creature;
			}
		}
	}

	[CustomPropertyDrawer(typeof(CreatureReference))]
	public class CreatureReferenceDrawer : ControlDrawer<CreatureReferenceControl>
	{
	}
}
