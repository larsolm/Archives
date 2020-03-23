using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CreateAssetMenu(fileName = "DefaultMaskTransition", menuName = "Monster Maker/Transitions/Mask Transition", order = 100)]
	public class MaskTransition : Transition
	{
		public Color MaskColor = Color.black;
		public Texture2D Mask;
		public Shader Shader;

		public override Texture2D GetTexture()
		{
			return Mask;
		}

		public override Shader GetShader()
		{
			return Shader;
		}

		public override void Start()
		{
			TransitionManager.Instance.Renderer.material.color = MaskColor;
		}

		public override void TransitionOut(float time)
		{
			var size = Mathf.Lerp(1.0f, 0.01f, time / DurationIn);

			TransitionManager.Instance.Renderer.material.SetFloat("_Size", size);
		}

		public override void Obscurred(float time)
		{
			TransitionManager.Instance.Renderer.material.SetFloat("_Size", 0.01f);
		}

		public override void TransitionIn(float time)
		{
			var size = Mathf.Lerp(0.01f, 1.0f, time / DurationIn);
			
			TransitionManager.Instance.Renderer.material.SetFloat("_Size", size);
		}
	}
}
