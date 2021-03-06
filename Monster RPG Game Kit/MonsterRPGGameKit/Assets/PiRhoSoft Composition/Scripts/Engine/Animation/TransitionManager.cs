﻿using System.Collections;
using System.Collections.Generic;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	[HelpURL(Composition.DocumentationUrl + "transition-manager")]
	public class TransitionManager : GlobalBehaviour<TransitionManager>
	{
		private const string _invalidAddWarning = "(CTMIA) this TransitionRenderer has already been added";
		private const string _invalidRemoveWarning = "(CTMIR) this TransitionRenderer has not been added";

		private RenderTexture _target;
		private List<TransitionRenderer> _renderers = new List<TransitionRenderer>();

		public Transition CurrentTransition { get; private set; }

		void Awake()
		{
			CreateCamera();
			CreateTarget();
		}

		void OnDestroy()
		{
			DestroyTarget();
		}

		private void CreateCamera()
		{
			var camera = GetComponent<Camera>();
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.cullingMask = 0;
			camera.depth = 1000;
		}

		private void CreateTarget()
		{
			_target = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);
		}

		private void DestroyTarget()
		{
			_target.Release();
			_target = null;
		}

		public void AddRenderer(TransitionRenderer renderer)
		{
			if (!_renderers.Contains(renderer))
				_renderers.Add(renderer);
			else
				Debug.LogWarning(_invalidAddWarning, renderer);
		}

		public void RemoveRenderer(TransitionRenderer renderer)
		{
			if (!_renderers.Remove(renderer))
				Debug.LogWarning(_invalidRemoveWarning, renderer);
		}

		public IEnumerator RunTransition(Transition transition, TransitionPhase phase)
		{
			yield return StartTransition(transition, phase);
			EndTransition();
		}

		public IEnumerator StartTransition(Transition transition, TransitionPhase phase)
		{
			EndTransition();

			if (transition)
			{
				if (_target.width != Screen.width || _target.height != Screen.height)
				{
					DestroyTarget();
					CreateTarget();
				}

				foreach (var renderer in _renderers)
					renderer.SetTarget(_target);

				CurrentTransition = transition;
				CurrentTransition.Begin(phase);

				for (var elapsed = 0.0f; elapsed < CurrentTransition.Duration; elapsed += Time.unscaledDeltaTime)
				{
					CurrentTransition.Process(elapsed, phase);
					yield return null;
				}
			}
		}

		public void EndTransition()
		{
			if (CurrentTransition)
			{
				foreach (var renderer in _renderers)
					renderer.SetTarget(null);

				CurrentTransition.End();
				CurrentTransition = null;
			}
		}

		void OnPostRender()
		{
			if (CurrentTransition)
				CurrentTransition.Render(_target, null);
		}
	}
}
