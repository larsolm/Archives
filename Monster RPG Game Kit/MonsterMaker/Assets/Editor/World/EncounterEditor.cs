using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(Encounter))]
	class EncounterEditor : Editor
	{
		private SerializedProperty _ecosystem;
		private SerializedProperty _instructions;
		private SerializedProperty _generationInstructions;
		private SerializedProperty _encounterChance;
		private EditableList<int> _encounterList = new EditableList<int>();
		private List<VariableStoreControl> _encounterControls = new List<VariableStoreControl>();
		private GenericMenu _addMenu;
		private int _toRemove;

		private void OnEnable()
		{
			var encounter = target as Encounter;
			if (encounter.Ecosystem)
			{
				SetupAddMenu(encounter.Ecosystem);
				
				foreach (var e in encounter.Encounters.Values)
					Definitions.Instance.EncounterTraits.Apply(e.Variables, false);
			}

			_ecosystem = serializedObject.FindProperty("Ecosystem");
			_instructions = serializedObject.FindProperty("Instructions");
			_generationInstructions = serializedObject.FindProperty("GenerationInstructions");
			_encounterChance = serializedObject.FindProperty("EncounterChance");

			var encounters = _encounterList.Setup(encounter.Encounters.Values, "Encounters", "The encounters that will be triggered for this area.", false, false, false, true, true, DrawEncounter, RemoveEncounter);
			encounters.onAddDropdownCallback += AddEncounterDropdown;
			encounters.elementHeightCallback += EncounterHeight;

			_encounterControls.Clear();
			foreach (var encounterType in encounter.Encounters.Values)
				_encounterControls.Add(new VariableStoreControl(encounterType.Variables, "Variables", "", false, false));
		}

		private void SetupAddMenu(Ecosystem ecosystem)
		{ 
			_addMenu = new GenericMenu();
			foreach (var species in ecosystem.Species)
				_addMenu.AddItem(new GUIContent(species.name, "The species to add."), false, AddEncounter, species);
		}

		private void DrawEncounter(Rect rect, int index)
		{
			var encounters = target as Encounter;
			var encounter = encounters.Encounters.GetValue(index);
			var weight = encounters.Encounters.GetWeight(index);
			var percent = encounters.Encounters.GetPercentageWeight(index);

			var nameRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
			var percentRect = new Rect(rect.xMax - (rect.width * 0.125f), rect.y, rect.width * 0.125f, EditorGUIUtility.singleLineHeight);
			var sliderRect = new Rect(nameRect.x + nameRect.width + 5, rect.y, rect.width - percentRect.width - nameRect.width - 10, EditorGUIUtility.singleLineHeight);
			var storeRect = new Rect(rect.x + 10, nameRect.yMax + 5, rect.width, EditorGUIUtility.singleLineHeight * (encounter.Variables.Variables.Count + 2));

			EditorGUI.LabelField(nameRect, encounter.Species.name);

			var selectedWeight = EditorGUI.IntSlider(sliderRect, weight, 1, encounters.Encounters.TotalWeight);

			EditorGUI.LabelField(percentRect, string.Format("({0:f1}%)", percent));
			_encounterControls[index].Draw(storeRect, null, GUIContent.none);

			if (weight != selectedWeight)
				encounters.Encounters.ChangeWeight(index, selectedWeight);
		}

		private float EncounterHeight(int index)
		{
			var encounters = target as Encounter;
			var encounter = encounters.Encounters.GetValue(index);

			return (encounter.Variables.Variables.Count + 3) * EditorGUIUtility.singleLineHeight + 20;
		}

		private void AddEncounterDropdown(Rect rect, ReorderableList list)
		{
			_addMenu.ShowAsContext();
		}

		private void RemoveEncounter(int index)
		{
			_toRemove = index;
		}

		public override void OnInspectorGUI()
		{
			var encounter = target as Encounter;

			_toRemove = -1;

			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_instructions);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_generationInstructions);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_encounterChance);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_ecosystem);
				EditorGUILayout.Space();

				if (_ecosystem.objectReferenceValue as Ecosystem != encounter.Ecosystem)
					EcosystemChanged();
			}

			if (encounter.Ecosystem)
			{
				using (new UndoScope(encounter))
					_encounterList.DrawList();
			}

			EditorGUILayout.Space();

			if (_toRemove >= 0)
				Remove(encounter, _toRemove);
		}

		private void AddEncounter(object data)
		{
			var encounter = target as Encounter;
			var species = data as Species;

			var encounterType = new EncounterType
			{
				Species = species,
				Variables = new VariableStore()
			};

			using (new UndoScope(encounter))
			{
				Definitions.Instance.EncounterTraits.Apply(encounterType.Variables, false);
				encounter.Encounters.Add(1, encounterType);
			}
		}

		private void Remove(Encounter encounter, int index)
		{
			using (new UndoScope(encounter))
				encounter.Encounters.Remove(index);
		}
		
		private void EcosystemChanged()
		{
			var encounter = target as Encounter;

			if (encounter.Ecosystem)
				SetupAddMenu(encounter.Ecosystem);

			encounter.Encounters.Clear();
		}
	}
}
