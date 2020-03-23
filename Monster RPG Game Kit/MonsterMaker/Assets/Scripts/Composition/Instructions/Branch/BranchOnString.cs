namespace PiRhoSoft.MonsterMaker
{
	public class BranchOnString : BranchInstruction<string>
	{
		public InstructionDictionary Instructions = new InstructionDictionary();

		protected override Instruction GetInstruction(string value)
		{
			Instruction instruction;
			Instructions.TryGetValue(value, out instruction);
			return instruction;
		}
	}
}
