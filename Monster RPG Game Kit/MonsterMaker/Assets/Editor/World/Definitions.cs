using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class Enumeration
	{
		public string Name;
		public List<string> Values = new List<string>();
	}

	public class Definitions : ScriptableObject
	{
		public VariableSchema TrainerTraits = new VariableSchema("Trainer Traits");
		public VariableSchema SpeciesTraits = new VariableSchema("Species Traits");
		public VariableSchema CreatureTraits = new VariableSchema("Creature Traits");
		public VariableSchema EncounterTraits = new VariableSchema("Encounter Traits");
		public VariableSchema MoveTraits = new VariableSchema("Move Traits");
		public List<VariableSchema> AbilityTraitsSchemas = new List<VariableSchema>();

		public static Definitions Instance
		{
			get
			{
				var assets = AssetFinder.GetMainAsset<Definitions>();

				if (assets == null)
				{
					assets = CreateInstance<Definitions>();
					AssetDatabase.CreateAsset(assets, "Assets/Definitions.asset");
					AssetDatabase.SaveAssets();
				}

				return assets;
			}
		}
	}
}

				//for (var i = 0; i < _abilityTraits.arraySize; i++)
				//{
				//	var abilityTrait = _abilityTraits.GetArrayElementAtIndex(i);
				//
				//	EditorGUILayout.Space();
				//	EditorGUILayout.PropertyField(abilityTrait);
				//}
				//
				//using (new EditorGUILayout.HorizontalScope())
				//{
				//	var enter = GuiFields.TextEnterField("NewTraitName", GUIContent.none, ref _newAbilityTraitsName);
				//	var click = GUILayout.Button("Create");
				//
				//	if ((enter || click) && !string.IsNullOrEmpty(_newAbilityTraitsName))
				//	{
				//		EditorAssets.Instance.AbilityTraitsSchemas.Add(new VariableSchema(_newAbilityTraitsName));
				//		_newAbilityTraitsName = "Ability Traits";
				//	}
				//}