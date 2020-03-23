using PiRhoSoft.CompositionEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public class MoveContextList : IndexedVariableStore<MoveContext> { }

	public class CreatureContext : LocalVariableStore<Creature>
	{
		public Creature Creature => _store;
		public MoveContextList Moves { get; private set; } = new MoveContextList();
		public TrainerContext Trainer { get; private set; } 

		public CreatureContext(BattleContext battle, TrainerContext trainer, Creature creature) : base(creature)
		{
			Trainer = trainer;

			foreach (var move in creature.Moves)
				Moves.Add(battle.CreateMove(this, move));
		}

		#region Variables

		public override VariableValue GetVariable(string name)
		{
			if (name == nameof(Moves)) return VariableValue.Create(Moves);
			else if (name == nameof(Trainer)) return VariableValue.Create(Trainer);
			else if (name == nameof(Creature)) return VariableValue.Create(Creature);
			else return base.GetVariable(name);
		}

		public override SetVariableResult SetVariable(string name, VariableValue value)
		{
			if (name == nameof(Moves)) return SetVariableResult.ReadOnly;
			else if (name == nameof(Trainer)) return SetVariableResult.ReadOnly;
			else if (name == nameof(Creature)) return SetVariableResult.ReadOnly;
			else return base.SetVariable(name, value);
		}

		#endregion
	}
}
