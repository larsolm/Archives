using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomEditor(typeof(Species))]
	class SpeciesEditor : Editor
	{
		private VariableStoreControl _traits;
		private SkillsControl _skills;

		private void OnEnable()
		{
			var species = (target as Species);
			Definitions.Instance.SpeciesTraits.Apply(species.Traits, true);
			
			_traits = new VariableStoreControl(species.Traits, "Traits", "", false, false);
			_skills = new SkillsControl(species);
		}

		public override void OnInspectorGUI()
		{
			var back = GUILayout.Button(EditorHelper.BackContent, GUILayout.Width(60.0f));
			
			EditorGUILayout.Space();

			using (new UndoScope(target))
			{
				_traits.Draw();
				EditorGUILayout.Space();
				_skills.Draw();
			}

			if (back)
				EditorHelper.Edit((target as Species).Ecosystem);
		}
	}
}
