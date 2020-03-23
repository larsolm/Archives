using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Managers/Transition Manager")]
	public class TransitionManager : Singleton<TransitionManager>
	{
		public const int Layer = 31;

		public Camera Camera { get; private set; }
		public MeshRenderer Renderer { get; private set; }
		public Transition CurrentTransition { get; private set; }
		public GameObject TransitionObject { get; private set; }
		public MeshFilter MeshFilter { get; private set; }

		private const float _cameraSize = 5.0f;

		private Texture2D _defaultTexture;
		private Shader _defaultShader;
		private Mesh _defaultQuad;

		protected override void OnAwake()
		{
			CreateQuad();
			CreateTexture();
			CreateDefaultShader();
			CreateObject();
		}

		public IEnumerator StartTransition(Transition transition)
		{
			while (CurrentTransition)
				yield return null;

			CurrentTransition = transition;
			CurrentTransition.Start();

			MeshFilter.mesh = transition.GetMesh() ?? _defaultQuad;
			Renderer.material.shader = transition.GetShader() ?? _defaultShader;
			Renderer.material.mainTexture = transition.GetTexture() ?? _defaultTexture;
			TransitionObject.SetActive(true);

			for (var elapsed = 0.0f; elapsed < CurrentTransition.DurationOut; elapsed += Time.unscaledDeltaTime)
			{
				CurrentTransition.TransitionOut(elapsed);
				yield return null;
			}
		}

		public IEnumerator ObscureTransition()
		{
			for (var elapsed = 0.0f; elapsed < CurrentTransition.ObscureTime; elapsed += Time.unscaledDeltaTime)
			{
				CurrentTransition.Obscurred(elapsed);
				yield return null;
			}
		}

		public IEnumerator FinishTransition()
		{
			for (var elapsed = 0.0f; elapsed < CurrentTransition.DurationIn; elapsed += Time.unscaledDeltaTime)
			{
				CurrentTransition.TransitionIn(elapsed);
				yield return null;
			}

			TransitionObject.SetActive(false);
			CurrentTransition.End();
			CurrentTransition = null;
		}

		private void CreateQuad()
		{
			var halfHeight = _cameraSize;
			var halfWidth = _cameraSize * (Screen.width / (float)Screen.height);

			_defaultQuad = new Mesh
			{
				vertices = new Vector3[]
				{
					new Vector3( -halfWidth, -halfHeight, 0 ),
					new Vector3( -halfWidth, halfHeight, 0 ),
					new Vector3( halfWidth, -halfHeight, 0 ),
					new Vector3( halfWidth, halfHeight, 0 )
				},

				uv = new Vector2[]
				{
					Vector2.zero,
					Vector2.up,
					Vector2.right,
					Vector2.one
				},

				triangles = new int[] { 0, 1, 2, 2, 1, 3 }
			};
		}

		private void CreateTexture()
		{
			_defaultTexture = new Texture2D(1, 1);
			_defaultTexture.SetPixel(0, 0, Color.white);
			_defaultTexture.Apply();
		}

		private void CreateDefaultShader()
		{
			_defaultShader = Shader.Find("Sprites/Default");
		}

		private void CreateObject()
		{
			TransitionObject = new GameObject("Transition");
			TransitionObject.transform.parent = transform;
			TransitionObject.layer = Layer;
			TransitionObject.hideFlags = HideFlags.HideInHierarchy;
			TransitionObject.SetActive(false);

			Camera = TransitionObject.AddComponent<Camera>();
			Camera.orthographic = true;
			Camera.orthographicSize = _cameraSize;
			Camera.nearClipPlane = -1.0f;
			Camera.farClipPlane = 1.0f;
			Camera.depth = float.MaxValue;
			Camera.clearFlags = CameraClearFlags.Nothing;
			Camera.cullingMask = 1 << Layer;

			MeshFilter = TransitionObject.AddComponent<MeshFilter>();
			Renderer = TransitionObject.AddComponent<MeshRenderer>();
		}
	}
}
