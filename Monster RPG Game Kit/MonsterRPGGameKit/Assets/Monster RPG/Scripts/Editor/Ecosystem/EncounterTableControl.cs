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
	public class EncounterTableControl : ObjectControl<EncounterTable>
	{
		private static readonly IconButton _removeButton = new IconButton(IconButton.Remove, "Remove this Encounter");
		private static readonly IconButton _addCreatureButton = new IconButton(IconButton.CustomAdd, "Add a Creature encounter that can be customized spefically in the editor");
		private static readonly IconButton _addSpeciesButton = new IconButton(IconButton.CustomAdd, "Add a Species encounter that creates a species when the encounter occurs");
		private static readonly GUIContent _emptyLabel = new GUIContent("The Encounter list is empty");

		private Object _target;
		private EncounterTable _table;
		private ObjectListControl _listControl = new ObjectListControl();
		private List<CreatureReferenceControl> _controls;
		private GenericMenu _addSpeciesMenu = new GenericMenu();
		private GenericMenu _addCreatureMenu = new GenericMenu();

		public override void Setup(EncounterTable target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_table = target;
			_target = property.serializedObject.targetObject;
			_controls = new List<CreatureReferenceControl>();

			_listControl.Setup(_table.Values)
				.MakeDrawable(DrawEncounter)
				.MakeRemovable(_removeButton, RemoveEncounter)
				.MakeCollapsable(property.serializedObject.targetObject.GetType() + "." + property.propertyPath + ".IsOpen")
				.MakeHeaderButton(_addSpeciesButton, _addSpeciesMenu, Color.white)
				.MakeHeaderButton(_addCreatureButton, _addCreatureMenu, Color.white)
				.MakeCustomHeight(GetEncounterHeight)
				.MakeEmptyLabel(_emptyLabel);

			var species = AssetHelper.GetAssetList<Species>(false, false);

			for (var i = 0; i < species.Assets.Count; i++)
			{
				if (species.Assets[i] != null)
				{
					_addSpeciesMenu.AddItem(species.Names[i], false, AddSpeciesEncounter, species.Assets[i]);
					_addCreatureMenu.AddItem(species.Names[i], false, AddCreatureEncounter, species.Assets[i]);
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

		private float GetEncounterHeight(int index)
		{
			var encounter = _table.GetValue(index);
			var height = GetControl(index, encounter).GetHeight(null);

			height += RectHelper.VerticalSpace + RectHelper.LineHeight;

			return height;
		}

		private void DrawEncounter(Rect rect, IList list, int index)
		{
			var encounter = _table.GetValue(index);
			var weight = _table.GetWeight(index);
			var percent = _table.GetPercentageWeight(index);

			var control = GetControl(index, encounter);
			var creatureHeight = control.GetHeight(null);
			var creatureRect = RectHelper.TakeHeight(ref rect, creatureHeight);

			control.Draw(creatureRect, null);

			RectHelper.TakeVerticalSpace(ref rect);
			RectHelper.TakeWidth(ref rect, rect.width * 0.25f);

			var percentRect = RectHelper.TakeTrailingWidth(ref rect, rect.width * 0.25f);
			var sliderRect = RectHelper.TakeWidth(ref rect, rect.width - RectHelper.HorizontalSpace);

			EditorGUI.LabelField(percentRect, string.Format("({0:f1}%)", percent));

			var selectedWeight = EditorGUI.IntSlider(sliderRect, weight, 1, _table.TotalWeight);
			if (weight != selectedWeight)
				_table.ChangeWeight(index, selectedWeight);
		}

		private void AddSpeciesEncounter(object data)
		{
			var species = data as Species;

			using (new UndoScope(_target, true))
				_table.Add(1, new CreatureReference { Species = species, Generator = new InstructionCaller() });
		}

		private void AddCreatureEncounter(object data)
		{
			var species = data as Species;
			var creature = species.CreateCreature(null);

			using (new UndoScope(_target, true))
			{
				_table.Add(1, new CreatureReference { Creature = creature });
				Undo.RegisterCreatedObjectUndo(creature, "Undo create");
			}
		}

		private void RemoveEncounter(IList list, int index)
		{
			_table.Remove(index);
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

	[CustomPropertyDrawer(typeof(EncounterTable))]
	public class EncounterTableDrawer : ControlDrawer<EncounterTableControl>
	{
	}
}
