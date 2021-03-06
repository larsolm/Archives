﻿using UnityEngine;

namespace PiRhoSoft.UtilityEngine
{
	public abstract class GlobalBehaviour<T> : MonoBehaviour where T : GlobalBehaviour<T>
	{
		private static T _instance;

		public static bool Exists => _instance != null;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject gameObject = new GameObject();
					_instance = gameObject.AddComponent<T>();
					gameObject.hideFlags = HideFlags.HideInHierarchy;
					DontDestroyOnLoad(gameObject);
				}

				return _instance;
			}
		}
	}
}
