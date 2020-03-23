namespace PiRhoSoft.MonsterMaker
{
	public class IterateTrainers : Instruction
	{
		public Instruction Instruction;
		private int _index;

		public override void Begin(InstructionContext context)
		{
			_index = 0;
		}

		public override bool Execute(InstructionContext context)
		{
			if (_index < BattleManager.Instance.Trainers.Count)
			{
				context.Run(Instruction, BattleManager.Instance.Trainers[_index]);
				_index++;
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
