using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CreateAssetMenu(fileName = "DefaultFadeTransition", menuName = "Monster Maker/Transitions/Fade Transition", order = 100)]
	public class FadeTransition : Transition
	{
		public Color FadeColor = Color.black;

		public override void TransitionOut(float time)
		{
			var alpha = Mathf.Lerp(0, FadeColor.a, time / DurationOut);

			TransitionManager.Instance.Renderer.material.color = new Color(FadeColor.r, FadeColor.g, FadeColor.b, alpha);
		}

		public override void Obscurred(float time)
		{
			TransitionManager.Instance.Renderer.material.color = FadeColor;
		}

		public override void TransitionIn(float time)
		{
			var alpha = Mathf.Lerp(FadeColor.a, 0, time / DurationIn);
			
			TransitionManager.Instance.Renderer.material.color = new Color(FadeColor.r, FadeColor.g, FadeColor.b, alpha);
		}
	}
}
