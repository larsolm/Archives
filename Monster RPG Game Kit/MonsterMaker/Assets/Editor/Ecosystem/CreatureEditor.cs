using System;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(Creature))]
	class CreatureEditor : Editor
	{
		private SerializedProperty _speciesProperty;
		private VariableStoreControl _traits;
		private EditableList<Skill> _skills = new EditableList<Skill>();
		private EditableList<Move> _moves = new EditableList<Move>();
		
		private EcosystemInstructionContext _context;
		private GUIContent _regenerate = new GUIContent("Regenerate", "Regenerate this creature based on the ecosystems instructions.");
		private GUIContent _apply = new GUIContent("Apply", "Run the on update instructions for this trait.");
		private GUIContent _applyDisabled = new GUIContent("Apply", "This trait has no on changed instructions.");

		private void OnEnable()
		{
			var creature = target as Creature;
			Definitions.Instance.CreatureTraits.Apply(creature.Traits, true);
			creature.CleanUpSkills();

			_speciesProperty = serializedObject.FindProperty("Species");

			_context = new EcosystemInstructionContext(creature, "Edit Creature", null);
			_traits = new VariableStoreControl(creature.Traits, "Traits", null, false, false, DrawTrait);
			var moves = _moves.Setup(creature.Moves, "Moves", "", false, true, false, true, true, DrawMove);
			_skills.Setup(creature.Species.Skills, "Skills", "", false, false, false, false, false, DrawSkill);

			moves.elementHeightCallback += GetMoveHeight;
			moves.onAddDropdownCallback += AddMove;

			_regenerate.image = EditorGUIUtility.IconContent("d_preAudioLoopOff").image;
			_apply.image = EditorGUIUtility.IconContent("d_preAudioLoopOff").image;
		}

		private void DrawTrait(Rect rect, int index)
		{
			var creature = target as Creature;
			var name = creature.Traits.Variables[index].Name;

			var variableRect = new Rect(rect.x, rect.y, rect.width * 0.8f, EditorGUIUtility.singleLineHeight);
			var applyRect = new Rect(variableRect.xMax + 5, variableRect.y, rect.width - variableRect.width - 5, variableRect.height);

			var hasInstruction = creature.Species.Ecosystem.CreatureUpdateInstructions.ContainsKey(name);

			_traits.DrawStoreEntry(variableRect, index);

			GUI.enabled = hasInstruction;

			if (GUI.Button(applyRect, hasInstruction ? _apply : _applyDisabled))
				EditorUpdater.StartCoroutine(creature.UpdateTraits(_context, name));

			GUI.enabled = true;
		}

		private float GetMoveHeight(int index)
		{
			var creature = target as Creature;
			var move = creature.Moves[index];

			return (EditorGUIUtility.singleLineHeight + 5) * (1 + move.Traits.Count);
		}

		private class AddMovePopup : PopupWindowContent
		{
			private CreatureEditor _editor;

			public AddMovePopup(CreatureEditor editor)
			{
				_editor = editor;
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, EditorGUIUtility.singleLineHeight * 4);
			}

			public override void OnGUI(Rect rect)
			{
				EditorGUILayout.LabelField(_label);

				GuiFields.SubAssetPopup<Ecosystem, Ability>(GUIContent.none, ref _newAbility);
				var create = GUILayout.Button(EditorHelper.CreateContent);

				if (create && _newAbility != null)
				{
					CreateSkill(_newAbility);
					editorWindow.Close();
				}
			}

			private GUIContent _label = new GUIContent("New Move", "Add a new move for the creature");
			private Ability _newAbility;

			private void CreateSkill(Ability ability)
			{
				(_editor.target as Creature).Moves.Add(new Move(ability));
			}
		}

		private void AddMove(Rect rect, ReorderableList list)
		{
			rect.y += EditorGUIUtility.singleLineHeight;
			PopupWindow.Show(rect, new AddMovePopup(this));
		}

		private void DrawMove(Rect rect, int index)
		{
			var creature = target as Creature;
			var move = creature.Moves[index];

			var labelRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
			var traitsRect = new Rect(rect.x + 10, labelRect.yMax + 5, rect.width - 10, EditorGUIUtility.singleLineHeight);

			EditorGUI.LabelField(labelRect, move.Ability.name);

			for (var i = 0; i < move.Traits.Count; i++)
			{
				var trait = move.Traits.GetVariable(i);
				var traitRect = EditorGUI.PrefixLabel(traitsRect, new GUIContent(trait.Name));
				VariableValueDrawer.DrawValue(traitRect, trait, null, null);
				traitsRect.y += EditorGUIUtility.singleLineHeight + 5;
			}
		}

		private void DrawSkill(Rect rect, int index)
		{
			var creature = target as Creature;
			var skill = creature.Species.Skills[index];
			var count = creature.LearnCount(skill);
			var showClear = creature.HasLearned(skill);
			var showLearn = creature.CanLearn(skill);
			var label = count == 0 ? "Unlearned" : (skill.LearnLimit == 1 ? "Learned" : string.Format("Learned {0} Times", count));

			var buttonSize = EditorGUIUtility.singleLineHeight;
			var buttonY = rect.y + (rect.height - buttonSize) * 0.5f;
			var labelWidth = rect.width * 0.7f - 5;

			labelWidth -= buttonSize + 4;
			labelWidth -= buttonSize + 4;

			var nameRect = new Rect(rect.x, rect.y, rect.width * 0.3f, rect.height);
			var labelRect = new Rect(nameRect.xMax + 5, rect.y, labelWidth, rect.height);
			var learnRect = new Rect(labelRect.xMax, buttonY, buttonSize, buttonSize);
			var clearRect = new Rect(learnRect.xMax + 4, buttonY, buttonSize, buttonSize);

			EditorGUI.LabelField(nameRect, skill.Name);
			EditorGUI.LabelField(labelRect, label);

			var learn = showLearn && GUI.Button(learnRect, "+");
			var clear = showClear && GUI.Button(clearRect, "x");

			if (learn) EditorUpdater.StartCoroutine(creature.TeachSkill(_context, skill));
			if (clear) creature.ForgetSkill(skill);

			if (learn || clear) EditorUtility.SetDirty(creature);
		}

		public override void OnInspectorGUI()
		{
			var creature = target as Creature;
			var regenerate = false;

			EditorGUILayout.PropertyField(_speciesProperty);
			EditorGUILayout.Space();
			
			if (creature.Species.Ecosystem.CreatureGenerationInstructions != null)
			{
				regenerate = GUILayout.Button(_regenerate);
				EditorGUILayout.Space();
			}

			using (new UndoScope(creature))
			{
				_traits.Draw();
				EditorGUILayout.Space();
				_moves.DrawList();
				EditorGUILayout.Space();
				_skills.DrawList();
			}

			if (regenerate)
				EditorUpdater.StartCoroutine(creature.GenerateTraits(_context));
		}
	}
}
