using System;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		public static T Instance { get; private set; }

		void Awake()
		{
			if (Instance != null)
			{
				Destroy(this);
				throw new UnityException(string.Format("Second instance of singleton class {0} was created.", GetType().ToString()));
			}

			Instance = this as T;

			OnAwake();
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnDestroy()
		{
			if (Instance == this)
				Instance = null;
		}
	}
}
