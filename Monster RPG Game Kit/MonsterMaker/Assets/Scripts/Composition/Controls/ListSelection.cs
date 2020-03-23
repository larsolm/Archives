using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Sample/UI/List Text Control")]
	class ListSelection : SelectionControl
	{
		[Tooltip("Whether or not wrapping of the selction is allowed.")] public bool Wrapping = false;
		[Tooltip("Whether or not this list is horizontal or not.")] public bool Horizontal = false;

		protected override void ItemsCreated()
		{
			gameObject.SetActive(true); // HORIBLE HACK TO MAKE THIS LAYOUT RIGHT AWAY BECAUSE FOR SOME REASON IT WAITS A FRAME AFTER THEY ARE CREATED AND IT CAN'T BE DONE IN ONENABLE.
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
			gameObject.SetActive(false);
		}

		public override void Left()
		{
			if (Horizontal)
				Previous();
		}

		public override void Right()
		{
			if (Horizontal)
				Next();
		}

		public override void Up()
		{
			if (!Horizontal)
				Previous();
		}

		public override void Down()
		{
			if (!Horizontal)
				Next();
		}

		private void Previous()
		{
			var selection = _currentSelection - 1;

			if (selection < 0)
				selection = Wrapping ? _options.Length - 1 : 0;

			UpdateSelection(selection);
		}

		private void Next()
		{
			var selection = _currentSelection + 1;

			if (selection == _options.Length)
				selection = Wrapping ? 0 : _options.Length - 1;

			UpdateSelection(selection);
		}
	}
}
