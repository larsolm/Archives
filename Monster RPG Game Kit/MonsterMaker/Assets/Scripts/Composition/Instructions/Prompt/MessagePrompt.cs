namespace PiRhoSoft.MonsterMaker
{
	public class MessagePrompt : PromptInstruction
	{
		public PromptString Message;

		public override void Begin(InstructionContext context)
		{
			context.Prompt(this, null);
		}

		public override void End(InstructionContext context)
		{
			context.Finish(this);
		}
	}
}
