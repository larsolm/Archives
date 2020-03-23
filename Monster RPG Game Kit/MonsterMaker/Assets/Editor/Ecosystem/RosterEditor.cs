using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(Roster))]
	public class RosterEditor : Editor
	{
		private GenericMenu _addMenu;
		private SerializedProperty _ecosystem;
		private SerializedProperty _creatures;
		private EditableList<Creature> _creatureList = new EditableList<Creature>();

		private EcosystemInstructionContext _context;

		private Creature _toRemove;
		private Creature _toEdit;

		private void OnEnable()
		{
			var roster = target as Roster;

			if (roster.Ecosystem)
				SetupAddMenu(roster.Ecosystem);

			_ecosystem = serializedObject.FindProperty("Ecosystem");
			_creatures = serializedObject.FindProperty("Creatures");

			var creatures = _creatureList.Setup(_creatures, null, null, true, true, false, true, true, null, RemoveCreature);
			creatures.onAddDropdownCallback += AddCreatureDropdown;
		}

		private void AddCreatureDropdown(Rect rect, ReorderableList list)
		{
			_addMenu.ShowAsContext();
		}

		private void RemoveCreature(SerializedProperty property, int index)
		{
			var roster = target as Roster;
			_toRemove = roster.Creatures[index];
		}

		private void DrawCreature(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			var creature = element.objectReferenceValue as Creature;
			var name = element.FindPropertyRelative("_name");
			var nameRect = new Rect(rect.x, rect.y, rect.width * 0.8f, EditorGUIUtility.singleLineHeight);
			var editRect = new Rect(nameRect.xMax + 5, rect.y, rect.width - nameRect.width - 5, nameRect.height);

			name.stringValue = EditorGUI.DelayedTextField(nameRect, creature.Name);
			if (name.stringValue == creature.Species.name)
				name.stringValue = "";

			if (GUI.Button(editRect, EditorHelper.EditContent))
				_toEdit = creature;
		}

		private void EcosystemChanged()
		{
			var roster = target as Roster;
			if (roster.Ecosystem)
				SetupAddMenu(roster.Ecosystem);

			_creatures.ClearArray();
		}

		private void SetupAddMenu(Ecosystem ecosystem)
		{
			_addMenu = new GenericMenu();
			foreach (var species in ecosystem.Species)
				_addMenu.AddItem(new GUIContent(species.name, "The species to add."), false, Add, species);
		}

		public override void OnInspectorGUI()
		{
			var roster = target as Roster;

			_toRemove = null;
			_toEdit = null;

			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.PropertyField(_ecosystem);
				EditorGUILayout.Space();

				if (_ecosystem.objectReferenceValue as Ecosystem != roster.Ecosystem)
					EcosystemChanged();

				if (roster.Ecosystem)
					_creatureList.DrawList();
			}

			if (_toRemove != null)
				Remove(roster, _toRemove);

			if (_toEdit != null)
				EditorHelper.Edit(_toEdit);
		}

		private void Add(object data)
		{
			var roster = target as Roster;
			var species = data as Species;
			var creature = Creature.Create(species);

			if (roster.Ecosystem.CreatureGenerationInstructions && (_context == null || !_context.IsRunning))
			{
				_context = new EcosystemInstructionContext(roster, "Edit Creature", null);
				EditorUpdater.StartCoroutine(creature.GenerateTraits(_context));
			}

			using (new UndoScope(roster))
			{
				Undo.RegisterCreatedObjectUndo(creature, "Undo create creature");
				roster.Creatures.Add(creature);
			}
		}

		private void Remove(Roster roster, Creature creature)
		{
			using (new UndoScope(roster))
			{
				roster.Creatures.Remove(creature);
				Undo.DestroyObjectImmediate(creature);
			}
		}
	}
}
