using PiRhoSoft.CompositionEngine;
using System.Collections;
using TMPro;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[AddComponentMenu("PiRho Soft/Interface/Typewriter Text")]
	[HelpURL(MonsterRpg.DocumentationUrl + "typewriter-control")]
	public class TypewriterControl : MessageControl
	{
		[Tooltip("The number of letters to type per second")]
		public float CharactersPerSecond = 25.0f;

		private int _currentPageCharacterCount;
		private float _characterDelay;

		protected override void Setup()
		{
			base.Setup();

			DisplayText.overflowMode = TextOverflowModes.Page;
		}

		protected override IEnumerator Run(string text, MessageInteractionType interaction, bool isLast)
		{
			var waitForPage = interaction == MessageInteractionType.WaitForInput || interaction == MessageInteractionType.WaitForFinalInput;

			_characterDelay = CharactersPerSecond == 0.0f ? 0.0f : 1.0f / CharactersPerSecond;
			_currentPageCharacterCount = 0;
			DisplayText.text = text;
			DisplayText.maxVisibleCharacters = 0;
			DisplayText.ForceMeshUpdate();

			yield return null; // consume the press that opened the message

			for (var page = 0; page < DisplayText.textInfo.pageCount; page++)
			{
				SetInteraction(MessageControlDisplay.None, interaction, isLast);

				yield return ShowPage(page);

				var lastPage = DisplayText.pageToDisplay == DisplayText.textInfo.pageCount;
				var wait = interaction == MessageInteractionType.WaitForInput || (lastPage && interaction == MessageInteractionType.WaitForFinalInput);

				if (wait)
					SetInteraction(lastPage ? MessageControlDisplay.Finished : MessageControlDisplay.Continue, interaction, isLast);

				while (wait && !InterfaceManager.Instance.Accept.Pressed)
					yield return null;

				yield return null; // consume the accept press
			}
		}

		private IEnumerator ShowPage(int index)
		{
			var page = DisplayText.textInfo.pageInfo[index];
			var delay = _characterDelay;

			DisplayText.maxVisibleCharacters = page.firstCharacterIndex;
			_currentPageCharacterCount += page.lastCharacterIndex - page.firstCharacterIndex + 1;
			DisplayText.pageToDisplay = index + 1;

			while (DisplayText.maxVisibleCharacters < _currentPageCharacterCount)
			{
				if (InterfaceManager.Instance.Cancel.Pressed)
				{
					// fast forward to the end of the text (one character per frame)
					_characterDelay = 0.0f;
					delay = 0.0f;
				}
				else if (InterfaceManager.Instance.Accept.Pressed)
				{
					// skip to the end of the page
					DisplayText.maxVisibleCharacters = _currentPageCharacterCount;
				}
				else if (delay < 0.0f)
				{
					delay += _characterDelay;
					DisplayText.maxVisibleCharacters++;
				}

				delay -= Time.deltaTime;
				yield return null;
			}
		}
	}
}
