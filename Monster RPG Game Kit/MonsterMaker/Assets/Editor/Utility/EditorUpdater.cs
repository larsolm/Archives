using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace PiRhoSoft.MonsterMaker
{
	[InitializeOnLoad]
	public static class EditorUpdater
	{
		private static List<IEnumerator> _coroutines = new List<IEnumerator>();

		static EditorUpdater()
		{
			EditorApplication.update += Update;
		}

		public static IEnumerator StartCoroutine(IEnumerator coroutine)
		{
			_coroutines.Add(coroutine);
			coroutine.MoveNext();
			return coroutine;
		}

		private static void Update()
		{
			for (var i = 0; i < _coroutines.Count; i++)
			{
				if (!_coroutines[i].MoveNext())
					_coroutines.RemoveAt(i--);
			}
		}
	}
}
