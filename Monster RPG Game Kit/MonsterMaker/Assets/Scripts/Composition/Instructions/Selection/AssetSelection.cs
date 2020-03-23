using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class AssetSelectionOption : SelectionOption
	{
		public ScriptableObject Selection;
	}

	public class AssetSelection : SelectionInstruction<AssetSelectionOption>
	{
		[Tooltip("The options to show and run")] public AssetSelectionOption[] AssetOptions;

		public override AssetSelectionOption[] Options { get { return AssetOptions; } }
	}
}
