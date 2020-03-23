using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Animator))]
	[HelpURL(MonsterRpg.DocumentationUrl + "battle-animation-events")]
	[AddComponentMenu("PiRho Soft/Interface/Battle Animation Events")]
	public class BattleAnimationEvents : MonoBehaviour
	{
		// dummy parameters exist to keep the functions from showing up in the AnimationClipEvent editor (since
		// functions with two parameters are not selectable).

		private const string _startClipWarning = "(BBAESC) failed to start clip: a clip named {0} has not been defined";
		private const string _playClipWarning = "(BBAEPC) failed to play clip: a clip named {0} has not been defined";
		private const string _startSoundWarning = "(BBAESS) failed to start sound: a sound named {0} has not been defined";
		private const string _playSoundWarning = "(BBAEPS) failed to play sound: a sound named {0} has not been defined";
		private const string _startEffectWarning = "(BBAESE) failed to start effect: an effect named {0} has not been defined";
		private const string _playEffectWarning = "(BBAEPE) failed to play effect: an effect named {0} has not been defined";
		private const string _pauseWarning = "(BBAEP) cannot pause the animator: the animator is already paused";
		private const string _unpauseWarning = "(BBAEU) cannot unpause the animator: the animator is not paused";

		[Tooltip("The animation clips available to play")] [DictionaryDisplay(ItemDisplay = ListItemDisplayType.Inline)] public BattleAnimationClipDictionary Clips = new BattleAnimationClipDictionary();
		[Tooltip("The sound clips available to play")] [DictionaryDisplay(ItemDisplay = ListItemDisplayType.Inline)] public BattleAnimationSoundDictionary Sounds = new BattleAnimationSoundDictionary();
		[Tooltip("The particle effects available to spawn")] [DictionaryDisplay(ItemDisplay = ListItemDisplayType.Inline)] public BattleAnimationEffectDictionary Effects = new BattleAnimationEffectDictionary();

		internal CreatureDisplay Display; // this is a field so it doesn't show up in the AnimationClipEvent editor

		private Animator _animator;
		private AnimationPlayer _animation;
		private float _animatorSpeed = 1.0f;
		private bool _isPaused = false;
		private Queue<QueuedEvent> _queue = new Queue<QueuedEvent>();

		void Awake()
		{
			_animator = GetComponent<Animator>();
			_animation = GetComponent<AnimationPlayer>();
		}

		#region Queue

		// Any events set on the same timeline frame, or any events encountered on the same render frame (which makes
		// this more likely when the game has a lower frame rate) will be triggered even if the animator is paused. So,
		// if the animator is paused, events need to be queued to run on unpause. This works great except for events
		// that pause on the last animation frame - there is no way to tell the animation to not complete.

		enum QueueType
		{
			StartClip,
			StartSound,
			StartEffect,
			PlayClip,
			PlaySound,
			PlayEffect
		}

		struct QueuedEvent
		{
			public string Name;
			public QueueType Type;
		}

		private bool Enqueue(string name, QueueType type)
		{
			if (_isPaused)
				_queue.Enqueue(new QueuedEvent { Name = name, Type = type });

			return _isPaused;
		}

		private void ProcessQueue()
		{
			while (!_isPaused && _queue.Count > 0)
			{
				var e = _queue.Dequeue();

				switch (e.Type)
				{
					case QueueType.StartClip: StartClip(e.Name); break;
					case QueueType.StartSound: StartSound(e.Name); break;
					case QueueType.StartEffect: StartEffect(e.Name); break;
					case QueueType.PlayClip: PlayClip(e.Name); break;
					case QueueType.PlaySound: PlaySound(e.Name); break;
					case QueueType.PlayEffect: PlayEffect(e.Name); break;
				}
			}
		}

		#endregion

		#region Clips

		public void StartClip(string name)
		{
			if (!Enqueue(name, QueueType.StartClip))
			{
				if (Clips.TryGetValue(name, out var clip))
					Display.Play(clip, Display.Variables);
				else
					Debug.LogWarningFormat(this, _startClipWarning, name);
			}
		}

		public void PlayClip(string name)
		{
			if (!Enqueue(name, QueueType.PlayClip))
			{
				if (Clips.TryGetValue(name, out var clip))
					StartCoroutine(RunClip(clip));
				else
					Debug.LogWarningFormat(this, _playClipWarning, name);
			}
		}

		private IEnumerator RunClip(BattleAnimationClip clip, bool dummy = false)
		{
			Pause();
			yield return Display.PlayAndWait(clip, Display.Variables);
			Unpause();
		}

		#endregion

		#region Sounds

		public void StartSound(string name)
		{
			if (!Enqueue(name, QueueType.StartSound))
			{
				if (Sounds.TryGetValue(name, out var sound))
					Display.Play(sound, Display.Variables);
				else
					Debug.LogWarningFormat(this, _startSoundWarning, name);
			}
		}
		
		public void PlaySound(string name)
		{
			if (!Enqueue(name, QueueType.PlaySound))
			{
				if (Sounds.TryGetValue(name, out var sound))
					StartCoroutine(RunSound(sound));
				else
					Debug.LogWarningFormat(this, _playSoundWarning, name);
			}
		}

		private IEnumerator RunSound(BattleAnimationSound sound, bool dummy = false)
		{
			Pause();
			yield return Display.PlayAndWait(sound, Display.Variables);
			Unpause();
		}

		#endregion

		#region Effects

		public void StartEffect(string name)
		{
			if (!Enqueue(name, QueueType.StartEffect))
			{
				if (Effects.TryGetValue(name, out var effect))
					Display.Play(effect, Display.Variables);
				else
					Debug.LogWarningFormat(this, _startEffectWarning, name);
			}
		}

		public void PlayEffect(string name)
		{
			if (!Enqueue(name, QueueType.PlayEffect))
			{
				if (Effects.TryGetValue(name, out var effect))
					StartCoroutine(RunEffect(effect));
				else
					Debug.LogWarningFormat(this, _playEffectWarning, name);
			}
		}

		private IEnumerator RunEffect(BattleAnimationEffect effect, bool dummy = false)
		{
			Pause();
			yield return Display.PlayAndWait(effect, Display.Variables);
			Unpause();
		}

		#endregion

		#region Playback

		protected void Pause(bool dummy1 = false, bool dummy2 = false)
		{
			if (!_isPaused)
			{
				_isPaused = true;
				_animatorSpeed = _animator.speed;

				if (_animation)
					_animation.Pause();
				else
					_animator.speed = 0.0f; // doing this when using an AnimationPlayer doesn't pause the clip
			}
			else
			{
				Debug.LogWarning(_pauseWarning, this);
			}
		}

		protected void Unpause(bool dummy1 = false, bool dummy2 = false)
		{
			if (_isPaused)
			{
				if (_animation)
					_animation.Unpause();
				else
					_animator.speed = _animatorSpeed;

				_isPaused = false;

				ProcessQueue();
			}
			else
			{
				Debug.LogWarning(_unpauseWarning, this);
			}
		}

		#endregion
	}
}
