namespace PiRhoSoft.MonsterMaker
{
	public class TeachAbilityToCreature : Instruction
	{
		[VariableType(typeof(Creature))] public VariableReference Creature = new VariableReference();
		[VariableType(typeof(Ability), typeof(Ecosystem))] public VariableReference Ability = new VariableReference();

		public override void Begin(InstructionContext context)
		{
			var creature = context.GetObject<Creature>(Creature);
			var ability = context.GetObject<Ability>(Ability);

			if (creature != null && ability != null)
				creature.Moves.Add(new Move(ability));
		}
	}
}
