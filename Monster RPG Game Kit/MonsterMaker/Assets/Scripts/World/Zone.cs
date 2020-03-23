using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class Zone : ScriptableObject
	{
		public World World;

		public string MapLayer;
		[Tooltip("The scene that this zone loads.")] public SceneReference Scene;
		[Tooltip("The background music to play in this zone.")] public AudioClip BackgroundMusic;
		[Tooltip("Whether or not to clamp a camera should clamp to the bounds of this zone.")] public bool ClampBounds;
		[Tooltip("Variables for this zone that cannot be changed in game.")] public VariableStore StaticState = new VariableStore();
		[Tooltip("Variables for this zone that can be changed in game and are persisted when the game is saved.")] public VariableStore DefaultState = new VariableStore();
	};
}
