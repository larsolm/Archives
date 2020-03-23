using System;
using System.Collections.Generic;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class BranchOnRange<T> : BranchInstruction<T> where T : IComparable<T>
	{
		[Serializable]
		public class Range
		{
			public T Value;
			public Instruction Instruction;
		}

		public List<Range> Instructions = new List<Range>();

		protected override Instruction GetInstruction(T value)
		{
			for (var i = 0; i < Instructions.Count; i++)
			{
				if (value.CompareTo(Instructions[i].Value) <= 0)
					return Instructions[i].Instruction;
			}

			return null;
		}
	}
}
