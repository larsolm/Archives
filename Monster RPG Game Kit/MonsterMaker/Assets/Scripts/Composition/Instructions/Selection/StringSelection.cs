using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public class StringSelectionOption : SelectionOption
	{
		public string Selection;
	}

	public class StringSelection : SelectionInstruction<StringSelectionOption>
	{
		[Tooltip("The options to show and run")] [SerializeField] public StringSelectionOption[] StringOptions;

		public override StringSelectionOption[] Options { get { return StringOptions; } }
	}
}
