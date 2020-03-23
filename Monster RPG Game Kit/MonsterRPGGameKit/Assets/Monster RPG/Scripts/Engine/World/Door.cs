using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[HelpURL(MonsterRpg.DocumentationUrl + "door")]
	[RequireComponent(typeof(AudioSource))]
	public class Door : AnimationPlayer
	{
		[Tooltip("The animation to play when the door opens")] public AnimationClip OpenAnimation;
		[Tooltip("The animation to play when the door closes")] public AnimationClip CloseAnimation;

		[Tooltip("The animation to play when the door opens")] public AudioClip OpenSound;
		[Tooltip("The animation to play when the door closes")] public AudioClip CloseSound;

		private AudioSource _audio;
		private List<Mover> _movers = new List<Mover>();
		private Vector2Int _tilePosition;

		protected override void Awake()
		{
			base.Awake();

			ComponentHelper.GetComponentsInScene(gameObject.scene.buildIndex, _movers, true);

			_tilePosition = Vector2Int.FloorToInt(transform.position);
			_audio = GetComponent<AudioSource>();
			_audio.outputAudioMixerGroup = AudioManager.Instance?.MasterMixer;
		}

		private void OnEnable()
		{
			if (Player.Instance)
				SetupCallbacks(Player.Instance.Mover);

			foreach (var mover in _movers)
				SetupCallbacks(mover);
		}

		private void OnDisable()
		{
			if (Player.Instance)
				TeardownCallbacks(Player.Instance.Mover);

			foreach (var mover in _movers)
				TeardownCallbacks(mover);
		}

		private void SetupCallbacks(Mover mover)
		{
			if (mover)
			{
				mover.OnWarp += Warped;
				mover.OnTileEntering += Entering;
				mover.OnTileExiting += Exiting;
				mover.OnTileChanged += Changed;
			}
		}

		private void TeardownCallbacks(Mover mover)
		{
			if (mover)
			{
				mover.OnWarp -= Warped;
				mover.OnTileEntering -= Entering;
				mover.OnTileExiting -= Exiting;
				mover.OnTileChanged -= Changed;
			}
		}

		private void Warped(Vector2Int position)
		{
			if (position == _tilePosition)
			{
				PlayAnimation(OpenAnimation);
				PlaySound(OpenSound);
			}
		}

		private void Entering(Vector2Int position)
		{
			if (position == _tilePosition)
			{
				PlayAnimation(OpenAnimation);
				PlaySound(OpenSound);
			}
		}

		private void Exiting(Vector2Int position)
		{
			if (position == _tilePosition)
			{
				PlayAnimation(CloseAnimation);
				PlaySound(CloseSound);
			}
		}

		private void Changed(Vector2Int from, Vector2Int to)
		{
			if (to == _tilePosition)
			{
				PlayAnimation(CloseAnimation);
				PlaySound(CloseSound);
			}
		}

		private void PlaySound(AudioClip sound)
		{
			if (sound)
				_audio.PlayOneShot(sound);
		}
	}
}
