using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Sample/UI/String Selection Item")]
	public class StringSelectionItem : SelectionItem
	{
		private TextMeshProUGUI _selectionText;
		private Image _selectionArrow;

		public override void Create(int index, object data)
		{
			var selection = data as StringSelectionOption;

			_selectionText = GetComponentInChildren<TextMeshProUGUI>();
			_selectionArrow = GetComponentInChildren<Image>();

			Debug.Assert(_selectionText, "A StringSelectionItem must have a child with a TextMeshProUGUI.", this);
			
			_selectionText.text = selection.Selection;
			_selectionText.GetComponent<RectTransform>().sizeDelta = _selectionText.GetPreferredValues();
		}

		public override void Focus()
		{
			if (_selectionArrow)
				_selectionArrow.enabled = true;
		}
		
		public override void Blur()
		{
			if (_selectionArrow)
				_selectionArrow.enabled = false;
		}
	}
}
