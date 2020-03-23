using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Sample/UI/Typewriter Text")]
	class TypewriterText : TextControl
	{
		[Tooltip("The image to show when the text has reached the end of the page.")] public Image ContinueImage = null;
		[Tooltip("How fast letters will appear.")] public float LettersPerSecond = 1.0f;
		
		private TextMeshProUGUI _text;

		private int _currentPageCharacterCount;
		private bool _pageFinished;
		private float _speed;

		public override void Cancel()
		{
			StopAllCoroutines();

			_text.maxVisibleCharacters = _text.textInfo.characterCount;
			_text.pageToDisplay = _text.textInfo.pageCount;
			_pageFinished = true;
			_finished = true;
			
			if (ContinueImage)
				ContinueImage.gameObject.SetActive(false);
		}

		protected override void Advance()
		{
			if (_pageFinished)
			{
				if (!_finished)
					StartPage(_text.pageToDisplay + 1);
				else
					base.Advance();
			}
			else
			{
				_text.maxVisibleCharacters = _currentPageCharacterCount;
			}
		}

		protected override void SetupText(string message)
		{
			_currentPageCharacterCount = 0;
			_speed = LettersPerSecond == 0.0f ? 0.0f : 1 / LettersPerSecond;
			_text.text = message;
		}

		protected override void Awake()
		{
			base.Awake();

			_text = GetComponentInChildren<TextMeshProUGUI>();

			Debug.Assert(_text, "A TypewriterText must have a child object with a TextMeshProUGUI component.", this);

			_text.enableWordWrapping = true;
			_text.overflowMode = TextOverflowModes.Page;
		}

		private void OnEnable()
		{
			StartPage(1);
		}

		private void StartPage(int index)
		{
			_text.ForceMeshUpdate();

			if (_text.textInfo.pageCount < index)
			{
				Cancel();
			}
			else
			{
				var page = _text.textInfo.pageInfo[index - 1];

				_text.maxVisibleCharacters = _currentPageCharacterCount;
				_currentPageCharacterCount += page.lastCharacterIndex - page.firstCharacterIndex + 1;
				_text.pageToDisplay = index;
				_pageFinished = false;

				if (ContinueImage)
					ContinueImage.gameObject.SetActive(false);

				StartCoroutine(AnimateText());
			}
		}

		private IEnumerator AnimateText()
		{
			while (_text.maxVisibleCharacters < _currentPageCharacterCount)
			{
				_text.maxVisibleCharacters++;

				yield return new WaitForSeconds(_speed);
			}

			_pageFinished = true;

			if (_text.pageToDisplay == _text.textInfo.pageCount)
				_finished = true;
			else if (ContinueImage)
				ContinueImage.gameObject.SetActive(true);
		}
	}
}
