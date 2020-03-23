using System.Collections.Generic;

namespace PiRhoSoft.MonsterMaker
{
	public class StartBattle : Instruction
	{
		public Battle Battle;
		public List<VariableReference> Trainers = new List<VariableReference>();
		public VariableReference Result = new VariableReference();

		private BattleData _data;

		public override void Begin(InstructionContext context)
		{
			var trainers = new List<Trainer>();

			foreach (var variable in Trainers)
			{
				var trainer = context.GetObject<Trainer>(variable);
				if (trainer != null)
					trainers.Add(trainer);
			}

			_data = BattleManager.Instance.StartBattle(Battle, trainers);
		}

		public override bool Execute(InstructionContext context)
		{
			return _data.Winner != null;
		}

		public override void End(InstructionContext context)
		{
			var store = context.GetStore(Result);
			var won = _data.Winner.GetComponent<Player>() == Player.Instance;

			if (store != null)
				store.SetOrAdd(Result.Name, won);
		}
	}
}
