using PiRhoSoft.CompositionEngine;
using System.Collections.Generic;

namespace PiRhoSoft.MonsterRpgEngine
{
	public class TrainerContextList : IndexedVariableStore<TrainerContext> { }

	public class BattleContext : VariableStore
	{
		public TrainerContextList Trainers { get; private set; } = new TrainerContextList();
		public TrainerContextList ActiveTrainers { get; private set; } = new TrainerContextList();

		public BattleContext(IList<ITrainer> trainers)
		{
			foreach (var trainer in trainers)
				Trainers.Add(CreateTrainer(trainer));

			foreach (var trainer in Trainers)
				ActiveTrainers.Add(trainer);
		}

		public virtual TrainerContext CreateTrainer(ITrainer trainer)
		{
			return new TrainerContext(this, trainer);
		}

		public virtual CreatureContext CreateCreature(TrainerContext trainer, Creature creature)
		{
			return new CreatureContext(this, trainer, creature);
		}

		public virtual MoveContext CreateMove(CreatureContext creature, Move move)
		{
			return new MoveContext(this, creature, move);
		}

		#region Variables

		public override VariableValue GetVariable(string name)
		{
			if (name == nameof(Trainers)) return VariableValue.Create(Trainers);
			else if (name == nameof(ActiveTrainers)) return VariableValue.Create(ActiveTrainers);
			else return base.GetVariable(name);
		}

		public override SetVariableResult SetVariable(string name, VariableValue value)
		{
			return name == nameof(Trainers) ? SetVariableResult.ReadOnly : base.SetVariable(name, value);
		}

		#endregion
	}
}
