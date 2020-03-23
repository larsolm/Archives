using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class BattleData
	{
		public Battle Battle;
		public VariableStore Traits = new VariableStore();
		public TrainerData[] Trainers;
		public Trainer Winner;

		public BattleData(Battle battle, TrainerData[] trainers)
		{
			Battle = battle;
			Trainers = trainers;
		}
	}

	public class TrainerData
	{
		public Trainer Trainer;
		public Roster Roster;
		public VariableStore Traits = new VariableStore();
		public CreatureData[] Creatures;
		public CreatureData ActiveCreature;

		public TrainerData(Trainer trainer, Roster roster, CreatureData[] creatures)
		{
			Trainer = trainer;
			Roster = roster;
			Creatures = creatures;
		}

		public void SetActiveCreature(CreatureData creature)
		{
			if (ActiveCreature != null)
				ActiveCreature.OnLeftBattle();

			ActiveCreature = creature;

			if (ActiveCreature != null)
				ActiveCreature.OnEnteredBattle();
		}
	}

	public class CreatureData
	{
		public Creature Creature;
		public VariableStore BattleTraits = new VariableStore();
		public VariableStore ActiveTraits = new VariableStore();

		public CreatureData(Creature creature)
		{
			Creature = creature;
		}

		public virtual void OnEnteredBattle()
		{
		}

		public virtual void OnLeftBattle()
		{
			ActiveTraits.Reset();
		}
	}

	[CreateAssetMenu(fileName = "Battle", menuName = "Monster Maker/Battle/Battle")]
	public class Battle : ScriptableObject
	{
		[Tooltip("Variables that can be read by instructions during this battle")] public VariableStore Traits = new VariableStore();
		[Tooltip("The instructions that define this type of battle.")] public InstructionCaller Instructions = new InstructionCaller();
		[Tooltip("The scene that will be loaded when this battle begins.")] public SceneReference BattleView;

		public virtual BattleData SetupBattle(List<Trainer> trainers)
		{
			var trainerDatas = new TrainerData[trainers.Count];

			for (var i = 0; i < trainerDatas.Length; i++)
				trainerDatas[i] = SetupTrainer(trainers[i]);

			return new BattleData(this, trainerDatas);
		}

		public virtual TrainerData SetupTrainer(Trainer trainer)
		{
			var roster = trainer.GetComponent<Roster>();
			var creatureDatas = new CreatureData[roster.Creatures.Count];

			for (var i = 0; i < creatureDatas.Length; i++)
				creatureDatas[i] = SetupCreature(roster.Creatures[i]);

			return new TrainerData(trainer, roster, creatureDatas);
		}

		public virtual CreatureData SetupCreature(Creature creature)
		{
			return new CreatureData(creature);
		}
	}
}
