using PiRhoSoft.CompositionEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public class MoveContext : LocalVariableStore<Move>
	{
		public Move Move => _store;
		public CreatureContext Creature { get; private set; }

		public MoveContext(BattleContext battle, CreatureContext creature, Move move) : base(move)
		{
			Creature = creature;
		}

		#region Variables

		public override VariableValue GetVariable(string name)
		{
			if (name == nameof(Creature)) return VariableValue.Create(Creature);
			else if (name == nameof(Move)) return VariableValue.Create(Move);
			else return base.GetVariable(name);
		}

		public override SetVariableResult SetVariable(string name, VariableValue value)
		{
			if (name == nameof(Creature)) return SetVariableResult.ReadOnly;
			else if (name == nameof(Move)) return SetVariableResult.ReadOnly;
			else return base.SetVariable(name, value);
		}

		#endregion
	}
}
