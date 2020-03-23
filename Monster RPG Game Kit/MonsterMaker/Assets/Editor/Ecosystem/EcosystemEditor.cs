using System.Collections.Generic;
using System.Linq;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(Ecosystem))]
	public class EcosystemEditor : Editor
	{
		private class AddSpeciesPopup : PopupWindowContent
		{
			public AddSpeciesPopup(EcosystemEditor editor)
			{
				_editor = editor;
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, EditorGUIUtility.singleLineHeight * 4);
			}

			public override void OnGUI(Rect rect)
			{
				EditorGUILayout.LabelField(_species);

				var enter = GuiFields.TextEnterField("NewSpeciesName", GUIContent.none, ref _newName);
				var create = GUILayout.Button(EditorHelper.CreateContent);

				if ((create || enter) && !string.IsNullOrEmpty(_newName))
				{
					var ecosystem = _editor.target as Ecosystem;
					_editor.AddSpecies(ecosystem, _newName);
					editorWindow.Close();
				}
			}
			
			private EcosystemEditor _editor;
			private GUIContent _species = new GUIContent("New Species", "Create new species.");
			private string _newName = "Name";
		}

		private class AddAbilityPopup : PopupWindowContent
		{
			public AddAbilityPopup(EcosystemEditor editor)
			{
				var ecosystem = editor.target as Ecosystem;

				_editor = editor;
				_schemas = Enumerable.Concat(Enumerable.Repeat("No Schema", 1), Definitions.Instance.AbilityTraitsSchemas.Select(schema => schema.Name)).ToArray();
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, EditorGUIUtility.singleLineHeight * 5);
			}

			public override void OnGUI(Rect rect)
			{
				EditorGUILayout.LabelField(_ability);

				var enter = GuiFields.TextEnterField("NewAbilityName", GUIContent.none, ref _newName);
				_newSchema = EditorGUILayout.Popup(_newSchema, _schemas);
				var create = GUILayout.Button(EditorHelper.CreateContent);

				if ((create || enter) && !string.IsNullOrEmpty(_newName))
				{
					var ecosystem = _editor.target as Ecosystem;
					var schema = _newSchema > 0 ? Definitions.Instance.AbilityTraitsSchemas[_newSchema - 1] : null;
					_editor.AddAbility(ecosystem, _newName, schema);
					editorWindow.Close();
				}
			}

			private EcosystemEditor _editor;
			private GUIContent _ability = new GUIContent("New Ability", "Create new ability.");
			private string _newName = "Name";
			private int _newSchema = 0;
			private string[] _schemas;
		}

		private class UpdateInstructionPopup : PopupWindowContent
		{
			private EcosystemEditor _editor;
			private GUIContent _label = new GUIContent("New Instruction", "Create new instruction for a trait.");
			private string[] _traitOptions;
			private int _selectedTrait = -1;

			public UpdateInstructionPopup(EcosystemEditor editor)
			{
				_editor = editor;

				var ecosystem = editor.target as Ecosystem;
				_traitOptions = Definitions.Instance.CreatureTraits.Definitions.Select(item => item.Name).Where(name => !ecosystem.CreatureUpdateInstructions.ContainsKey(name)).ToArray();
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, EditorGUIUtility.singleLineHeight * 4);
			}

			public override void OnGUI(Rect rect)
			{
				EditorGUILayout.LabelField(_label);

				_selectedTrait = EditorGUILayout.Popup(_selectedTrait, _traitOptions);

				if (GUILayout.Button(EditorHelper.CreateContent) && _selectedTrait >= 0)
				{
					_editor.AddInstruction(_editor.target as Ecosystem, _traitOptions[_selectedTrait]);
					editorWindow.Close();
				}
			}
		}

		private EditableList<Species> _species = new EditableList<Species>();
		private EditableList<Ability> _abilities = new EditableList<Ability>();
		private EditableList<string> _updateInstructions = new EditableList<string>();

		private List<string> _updateInstructionKeys = new List<string>();

		private SerializedProperty _generationInstructions;
		private string _instructionToRemove;
		private Species _speciesToRemove;
		private Species _speciesToEdit;
		private Ability _abilityToRemove;
		private Ability _abilityToEdit;

		private GUIContent _updateInstructionLabel = new GUIContent("", "The instruction to run when this trait changes.");

		private void OnEnable()
		{
			var ecosystem = target as Ecosystem;

			_generationInstructions = serializedObject.FindProperty("CreatureGenerationInstructions");

			var species = _species.Setup(serializedObject.FindProperty("Species"), null, null, true, true, false, true, true, DrawSpecies, RemoveSpecies);
			species.onAddDropdownCallback += AddSpeciesDropdown;

			var abilities = _abilities.Setup(serializedObject.FindProperty("Abilities"), null, null, true, true, false, true, true, DrawAbility, RemoveAbility);
			abilities.onAddDropdownCallback += AddAbilityDropdown;
			
			UpdateInstructionKeys(ecosystem);

			var instructions = _updateInstructions.Setup(_updateInstructionKeys, "Creature Update Instructions", "The instructions to run when a creature trait is changed.", true, false, false, true, true, DrawInstruction, RemoveInstruction);
			instructions.onAddDropdownCallback += AddInstructionDropdown;
		}

		private void DrawInstruction(Rect rect, int index)
		{
			var ecosystem = target as Ecosystem;
			var key = _updateInstructionKeys[index];
			var instruction = ecosystem.CreatureUpdateInstructions[key];

			var keyRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
			var instructionRect = new Rect(keyRect.xMax + 5, rect.y, rect.width - keyRect.width - 5, keyRect.height);

			EditorGUI.LabelField(keyRect, key);

			ecosystem.CreatureUpdateInstructions[key] = InstructionDrawer.Draw(instructionRect, instruction == null ? null : instruction.Set, instruction, _updateInstructionLabel);
		}

		private void RemoveInstruction(int index)
		{
			_instructionToRemove = _updateInstructionKeys[index];
		}

		private void AddInstructionDropdown(Rect rect, ReorderableList list)
		{
			rect.y += EditorGUIUtility.singleLineHeight;
			PopupWindow.Show(rect, new UpdateInstructionPopup(this));
		}

		private void DrawSpecies(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			var species = element.objectReferenceValue as Species;
			var nameRect = new Rect(rect.x, rect.y, rect.width * 0.8f, EditorGUIUtility.singleLineHeight);
			var editRect = new Rect(nameRect.xMax + 5, rect.y, rect.width - nameRect.width - 5, nameRect.height);
			
			using (new UndoScope(species))
				species.name = EditorGUI.DelayedTextField(nameRect, species.name);

			if (GUI.Button(editRect, EditorHelper.EditContent))
				_speciesToEdit = species;
		}

		private void RemoveSpecies(SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			_speciesToRemove = element.objectReferenceValue as Species;
		}

		private void AddSpeciesDropdown(Rect rect, ReorderableList list)
		{
			rect.y += EditorGUIUtility.singleLineHeight;
			PopupWindow.Show(rect, new AddSpeciesPopup(this));
		}

		private void DrawAbility(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			var ability = element.objectReferenceValue as Ability;
			var nameRect = new Rect(rect.x, rect.y, rect.width * 0.8f, EditorGUIUtility.singleLineHeight);
			var editRect = new Rect(nameRect.xMax + 5, rect.y, rect.width * 0.2f - 5, nameRect.height);

			using (new UndoScope(ability))
				ability.name = EditorGUI.DelayedTextField(nameRect, ability.name);

			if (GUI.Button(editRect, EditorHelper.EditContent))
				_abilityToEdit = ability;
		}

		private void RemoveAbility(SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			_abilityToRemove = element.objectReferenceValue as Ability;
		}

		private void AddAbilityDropdown(Rect rect, ReorderableList list)
		{
			rect.y += EditorGUIUtility.singleLineHeight;
			PopupWindow.Show(rect, new AddAbilityPopup(this));
		}

		public override void OnInspectorGUI()
		{
			var ecosystem = target as Ecosystem;

			_instructionToRemove = null;
			_speciesToRemove = null;
			_speciesToEdit = null;
			_abilityToRemove = null;
			_abilityToEdit = null;

			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.PropertyField(_generationInstructions);
				EditorGUILayout.Space();
			}

			using (new UndoScope(target))
			{
				_updateInstructions.DrawList();
				EditorGUILayout.Space();
			}

			using (new UndoScope(serializedObject))
			{
				_species.DrawList();
				EditorGUILayout.Space();
			}

			using (new UndoScope(serializedObject))
				_abilities.DrawList();

			if (!string.IsNullOrEmpty(_instructionToRemove))
				Remove(ecosystem, _instructionToRemove);

			if (_speciesToRemove)
				Remove(ecosystem, _speciesToRemove);

			if (_speciesToEdit)
				Edit(_speciesToEdit);

			if (_abilityToRemove)
				Remove(ecosystem, _abilityToRemove);

			if (_abilityToEdit)
				Edit(_abilityToEdit);
		}

		private void AddInstruction(Ecosystem ecosystem, string trait)
		{
			using (new UndoScope(ecosystem))
				ecosystem.CreatureUpdateInstructions.Add(trait, null);

			UpdateInstructionKeys(ecosystem);
		}

		private void Remove(Ecosystem ecosystem, string instruction)
		{
			using (new UndoScope(ecosystem))
				ecosystem.CreatureUpdateInstructions.Remove(instruction);

			UpdateInstructionKeys(ecosystem);
		}

		private void UpdateInstructionKeys(Ecosystem ecosystem)
		{
			_updateInstructionKeys.Clear();

			foreach (var key in ecosystem.CreatureUpdateInstructions.Keys)
				_updateInstructionKeys.Add(key); // DON'T USE LINQ AS IT WILL REINSTANTIATE THE LIST AND THE ORIGINAL REFERENCE WILL BE BROKEN.
		}

		private void AddSpecies(Ecosystem ecosystem, string name)
		{
			var species = CreateInstance<Species>();
			var existingNames = ecosystem.Species.Select(i => i.name).ToArray();
			species.hideFlags = HideFlags.HideInHierarchy;
			species.name = ObjectNames.GetUniqueName(existingNames, name);
			species.Ecosystem = ecosystem;
			Definitions.Instance.SpeciesTraits.Apply(species.Traits, false);

			using (new UndoScope(ecosystem))
			{
				ecosystem.Species.Add(species);

				Undo.RegisterCreatedObjectUndo(species, "Undo create species");
				AssetDatabase.AddObjectToAsset(species, ecosystem);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ecosystem));
			}
		}

		private void AddAbility(Ecosystem ecosystem, string name, VariableSchema schema)
		{
			var ability = CreateInstance<Ability>();
			var existingNames = ecosystem.Abilities.Select(i => i.name).ToArray();
			ability.hideFlags = HideFlags.HideInHierarchy;
			ability.name = ObjectNames.GetUniqueName(existingNames, name);
			ability.Ecosystem = ecosystem;

			if (schema != null)
				schema.Apply(ability.Traits, false);

			using (new UndoScope(ecosystem))
			{
				ecosystem.Abilities.Add(ability);

				Undo.RegisterCreatedObjectUndo(ability, "Undo create ability");
				AssetDatabase.AddObjectToAsset(ability, ecosystem);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ecosystem));
			}
		}

		private void Remove(Ecosystem ecosystem, Species species)
		{
			using (new UndoScope(ecosystem))
			{
				ecosystem.Species.Remove(species);

				Undo.DestroyObjectImmediate(species);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ecosystem));
			}
		}

		private void Edit(Species species)
		{
			EditorHelper.Edit(species);
		}
		
		private void Remove(Ecosystem ecosystem, Ability ability)
		{
			using (new UndoScope(ecosystem))
			{
				ecosystem.Abilities.Remove(ability);
				Undo.DestroyObjectImmediate(ability);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ecosystem));
			}
		}

		private void Edit(Ability ability)
		{
			EditorHelper.Edit(ability);
		}
	}
}
