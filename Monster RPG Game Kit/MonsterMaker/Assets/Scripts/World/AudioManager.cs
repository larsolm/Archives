﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/World/Audio Manager")]
	public class AudioManager : Singleton<AudioManager>
	{
		public AudioMixerGroup MasterMixer;

		public AudioClip CurrentMusic { get { return _clips.Peek(); } }

		private Stack<AudioClip> _clips = new Stack<AudioClip>();
		private AudioSource[] _sources = new AudioSource[4];
		private Coroutine[] _routines  = new Coroutine[4];

		private int _currentIndex = 0;

		void Start()
		{
			for (var i = 0; i < _sources.Length; i++)
				_sources[i] = CreateSource();

			if (WorldManager.Instance.World.BackgroundMusic)
				Push(WorldManager.Instance.World.BackgroundMusic, 0.0f, 1.0f, 0.0f);
		}

		public void Push(AudioClip clip, float fadeOut = 0, float fadeIn = 0, float crossFade = 0)
		{
			_clips.Push(clip);
			StartCoroutine(Fade(clip, fadeOut, fadeIn, crossFade));
		}
		
		public void Pop(float fadeOut = 0, float fadeIn = 0, float crossFade = 0)
		{
			_clips.Pop();
			StartCoroutine(Fade(_clips.Peek(), fadeOut, fadeIn, crossFade));
		}

		private AudioSource CreateSource()
		{
			var source = gameObject.AddComponent<AudioSource>();
			source.outputAudioMixerGroup = MasterMixer;
			source.loop = true;
			source.volume = 0;
			return source;
		}

		private int GetSourceIndex(int index)
		{
			return index % _sources.Length;
		}

		private IEnumerator Fade(AudioClip clip, float fadeOut, float fadeIn, float crossFade)
		{
			var fromIndex = GetSourceIndex(_currentIndex);
			var toIndex = GetSourceIndex(++_currentIndex);

			var fromSource = _sources[fromIndex];
			var toSource = _sources[toIndex];

			var fromRoutine = _routines[fromIndex];
			var toRountine = _routines[toIndex];

			toSource.clip = clip;
			toSource.Play();

			if (fromRoutine != null)
				StopCoroutine(fromRoutine);

			if (toRountine != null)
				StopCoroutine(toRountine);

			_routines[fromIndex] = StartCoroutine(Fade(fromSource, fadeOut, 0.0f, true));

			if (fadeOut - crossFade > 0)
				yield return new WaitForSeconds(fadeOut - crossFade);

			_routines[toIndex] = StartCoroutine(Fade(toSource, fadeIn, 1.0f, false));
		}

		private IEnumerator Fade(AudioSource source, float duration, float to, bool stop)
		{
			var elapsed = 0.0f;
			var from = source.volume;

			while (source.volume != to)
			{
				elapsed += Time.deltaTime;
				var time = duration == 0.0f ? 1.0f : Mathf.Clamp01(elapsed / duration);
				source.volume = Mathf.Lerp(from, to, time);
				yield return null;
			}

			if (stop)
				source.Stop();
		}
	}
}
