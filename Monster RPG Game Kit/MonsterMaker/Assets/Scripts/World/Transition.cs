using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class Transition : ScriptableObject
	{
		public float DurationOut = 1.0f;
		public float ObscureTime = 1.0f;
		public float DurationIn = 1.0f;

		public virtual Shader GetShader() { return null; }
		public virtual Mesh GetMesh() { return null; }
		public virtual Texture2D GetTexture() { return null; }

		public virtual void Start() {}
		public virtual void TransitionOut(float time) {}
		public virtual void Obscurred(float time) {}
		public virtual void TransitionIn(float time) {}
		public virtual void End() {}
	}
}
