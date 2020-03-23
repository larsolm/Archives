using PiRhoSoft.CompositionEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	public class CreatureContextList : IndexedVariableStore<CreatureContext> { }

	public class TrainerContext : LocalVariableStore<ITrainer>
	{
		public ITrainer Trainer => _store;
		public CreatureContextList Creatures { get; private set; } = new CreatureContextList();

		public TrainerContext(BattleContext battle, ITrainer trainer) : base(trainer)
		{
			for (var i = 0; i < trainer.Roster.Count; i++)
			{
				var creature = battle.CreateCreature(this, trainer.Roster[i].Creature);
				Creatures.Add(creature);
			}
		}

		#region Variables

		public override VariableValue GetVariable(string name)
		{
			if (name == nameof(Creatures)) return VariableValue.Create(Creatures);
			else if (name == nameof(Trainer)) return VariableValue.Create(Trainer);
			else return base.GetVariable(name);
		}

		public override SetVariableResult SetVariable(string name, VariableValue value)
		{
			if (name == nameof(Creatures)) return SetVariableResult.ReadOnly;
			else if (name == nameof(Trainer)) return SetVariableResult.ReadOnly;
			else return base.SetVariable(name, value);
		}

		#endregion

		#region Selections
		/*
		public IEnumerator SelectCreature(Selection<BattleCreatureData> selection, bool isRequired)
		{
			if (Ai)
				Ai.SelectCreature(Battle, selection, isRequired);
			else
				yield return selection.Show(Battle, isRequired);
		}

		public IEnumerator SelectAction(Selection<BattleActionData> selection, bool isRequired)
		{
			if (Ai)
				Ai.SelectAction(Battle, selection, isRequired);
			else
				yield return selection.Show(Battle, isRequired);
		}

		public IEnumerator SelectMove(Selection<BattleMoveData> selection, bool isRequired)
		{
			if (Ai)
				Ai.SelectMove(Battle, selection, isRequired);
			else
				yield return selection.Show(Battle, isRequired);
		}

		public IEnumerator SelectItem(Selection<InventoryItem> selection, bool isRequired)
		{
			if (Ai)
				Ai.SelectItem(Battle, selection, isRequired);
			else
				yield return selection.Show(Battle, isRequired);
		}

		public IEnumerator Select<ItemType>(Selection<ItemType> selection, bool isRequired) where ItemType : class, IVariableStore
		{
			if (Ai)
				Ai.Select(Battle, selection, isRequired);
			else
				yield return selection.Show(Battle, isRequired);
		}
		*/

		#endregion
	}
}
