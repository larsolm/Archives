using System;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.MonsterMaker
{
	[Serializable]
	public struct SceneReference
	{
		public string Path;
		public Scene Scene { get { return SceneManager.GetSceneByPath(Path); } }
		public int Index { get { return SceneUtility.GetBuildIndexByScenePath(Path); } }
	}
}
