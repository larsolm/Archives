using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEditor
{
	public class RosterControl : ObjectControl<Roster>
	{
		private static readonly IconButton _addCreatureButton = new IconButton(IconButton.CustomAdd, "Add a Creature that can be customized in the editor");
		private static readonly IconButton _addSpeciesButton = new IconButton(IconButton.CustomAdd, "Add a Species to be created when the game loads");
		private static readonly IconButton _removeButton = new IconButton(IconButton.Remove, "Remove this Creature");
		private static readonly GUIContent _emptyLabel = new GUIContent("The Roster is empty");

		private Object _owner;
		private Roster _roster;
		private ObjectListControl _listControl = new ObjectListControl();
		private List<CreatureReferenceControl> _controls;
		private GenericMenu _addSpeciesMenu = new GenericMenu();
		private GenericMenu _addCreatureMenu = new GenericMenu();

		public override void Setup(Roster target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_roster = target;
			_owner = property.serializedObject.targetObject;
			_controls = new List<CreatureReferenceControl>();

			_listControl.Setup(_roster)
				.MakeDrawable(DrawCreature)
				.MakeRemovable(_removeButton)
				.MakeCollapsable(property.serializedObject.targetObject.GetType() + "." + property.propertyPath + ".IsOpen")
				.MakeReorderable()
				.MakeHeaderButton(_addSpeciesButton, _addSpeciesMenu, Color.white)
				.MakeHeaderButton(_addCreatureButton, _addCreatureMenu, Color.white)
				.MakeCustomHeight(GetCreatureHeight)
				.MakeEmptyLabel(_emptyLabel);
			
			var species = AssetHelper.GetAssetList<Species>(false, false);

			for (var i = 0; i < species.Assets.Count; i++)
			{
				if (species.Assets[i] != null)
				{
					_addSpeciesMenu.AddItem(species.Names[i], false, AddSpecies, species.Assets[i]);
					_addCreatureMenu.AddItem(species.Names[i], false, AddCreature, species.Assets[i]);
				}
			}
		}

		private CreatureReferenceControl GetControl(int index, CreatureReference creature)
		{
			while (index >= _controls.Count)
				_controls.Add(new CreatureReferenceControl());

			if (_controls[index].Creature != creature)
				_controls[index].Setup(creature, null, null, null);

			return _controls[index];
		}

		private float GetCreatureHeight(int index)
		{
			var reference = _roster[index];
			var height = 0.0f;

			if (reference.Species != null)
				height += GetControl(index, reference).GetHeight(null);

			if (reference.Species != null && reference.Creature != null)
				height += RectHelper.VerticalSpace;

			if (reference.Creature != null)
				height += EditorGUIUtility.singleLineHeight;

			return height;
		}

		private void DrawCreature(Rect rect, IList list, int index)
		{
			var reference = _roster[index];
			var control = GetControl(index, reference);

			control.Draw(rect, null);
		}

		private void AddSpecies(object data)
		{
			var species = data as Species;

			using (new UndoScope(_owner, true))
				_roster.Add(new CreatureReference { Species = species, Generator = new InstructionCaller() });
		}

		private void AddCreature(object data)
		{
			var species = data as Species;
			var creature = species.CreateCreature(_owner as ITrainer);

			using (new UndoScope(_owner, true))
			{
				_roster.Add(new CreatureReference { Creature = creature });
				Undo.RegisterCreatedObjectUndo(creature, "Undo create");
			}
		}

		public override float GetHeight(GUIContent label)
		{
			return _listControl.GetHeight();
		}

		public override void Draw(Rect position, GUIContent label)
		{
			_listControl.Draw(position, label);
		}
	}

	[CustomPropertyDrawer(typeof(Roster))]
	public class RosterDrawer : ControlDrawer<RosterControl>
	{
	}
}
