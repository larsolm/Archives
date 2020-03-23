using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[HelpURL(MonsterRpg.DocumentationUrl + "ecosystem")]
	[CreateAssetMenu(menuName = "PiRho Soft/Ecosystem", fileName = nameof(Ecosystem), order = 200)]
	public class Ecosystem : ScriptableObject
	{
		public const string InBattle = "In Battle";
		public const string ActiveInBattle = "Active In Battle";

		[Tooltip("The schema used for variables given to trainers")]
		[VariableInitializer(VariableInitializerType.Expression)]
		[VariableAvailabilities(VariableDefinition.NotSaved, VariableDefinition.Saved, InBattle, ActiveInBattle)]
		public VariableSchema TrainerSchema = new VariableSchema();

		[Tooltip("The schema used for variables given to species")]
		[VariableInitializer(VariableInitializerType.DefaultValue)]
		public VariableSchema SpeciesSchema = new VariableSchema();

		[Tooltip("The schema used for variables given to creatures")]
		[VariableInitializer(VariableInitializerType.Expression)]
		[VariableAvailabilities(VariableDefinition.NotSaved, VariableDefinition.Saved, InBattle, ActiveInBattle)]
		public VariableSchema CreatureSchema = new VariableSchema();

		[Tooltip("The schema used for variables given to abilities")]
		[VariableInitializer(VariableInitializerType.DefaultValue)]
		public VariableSchema AbilitySchema = new VariableSchema();

		[Tooltip("The schema used for variables given to moves")]
		[VariableInitializer(VariableInitializerType.Expression)]
		[VariableAvailabilities(VariableDefinition.NotSaved, VariableDefinition.Saved, InBattle, ActiveInBattle)]
		public VariableSchema MoveSchema = new VariableSchema();

		private VariableMap _trainerMap;
		private VariableMap _speciesMap;
		private VariableMap _creatureMap;
		private VariableMap _abilityMap;
		private VariableMap _moveMap;

		#region Map Access

		public VariableMap GetTrainerMap(PropertyMap propertyMap)
		{
			if (_trainerMap == null || _trainerMap.Version != TrainerSchema.Version)
				_trainerMap = new VariableMap(TrainerSchema.Version).Add(propertyMap).Add(TrainerSchema);

			return _trainerMap;
		}

		public VariableMap GetSpeciesMap(PropertyMap propertyMap)
		{
			if (_speciesMap == null || _speciesMap.Version != SpeciesSchema.Version)
				_speciesMap = new VariableMap(SpeciesSchema.Version).Add(propertyMap).Add(SpeciesSchema);

			return _speciesMap;
		}

		public VariableMap GetCreatureMap(PropertyMap propertyMap)
		{
			if (_creatureMap == null || _creatureMap.Version != CreatureSchema.Version)
				_creatureMap = new VariableMap(CreatureSchema.Version).Add(propertyMap).Add(CreatureSchema);

			return _creatureMap;
		}

		public VariableMap GetAbilityMap(PropertyMap propertyMap)
		{
			if (_abilityMap == null || _abilityMap.Version != AbilitySchema.Version)
				_abilityMap = new VariableMap(AbilitySchema.Version).Add(propertyMap).Add(AbilitySchema);

			return _abilityMap;
		}

		public VariableMap GetMoveMap(PropertyMap propertyMap)
		{
			if (_moveMap == null || _moveMap.Version != MoveSchema.Version)
				_moveMap = new VariableMap(MoveSchema.Version).Add(propertyMap).Add(MoveSchema);

			return _moveMap;
		}

		#endregion
	}
}
