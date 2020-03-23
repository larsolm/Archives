using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[HelpURL(MonsterRpg.DocumentationUrl + "zone")]
	[CreateAssetMenu(fileName = nameof(Zone), menuName = "PiRho Soft/Zone", order = 301)]
	public class Zone : ScriptableObject
	{
		public const string ZoneLoadedAvailability = "Zone is Loaded";
		public const string ZoneActiveAvailability = "Zone is Active";

		[Tooltip("The World this Zone is a part of")]
		public World World;

		[Tooltip("The name of this zone")]
		public string Name;

		[Tooltip("The scene that this zone loads")]
		public SceneReference Scene = new SceneReference();

		[Tooltip("The map layer this zone is in")]
		public string MapLayer;

		[Tooltip("The music to play when the player is in this zone")]
		public AudioClip BackgroundMusic;

		[Tooltip("Defines the variables available on the Zone")]
		[VariableAvailabilities(VariableDefinition.NotSaved, VariableDefinition.Saved, ZoneLoadedAvailability, ZoneActiveAvailability)]
		public VariableSchema Schema = new VariableSchema();

		[Tooltip("The instructions to run when this zone is fully entered")] public InstructionCaller EnterInstructions;
		[Tooltip("The instructions to run when this zone is about to be exited")] public InstructionCaller ExitInstructions;

		#region Scene Maintenance

		void OnEnable()
		{
			Scene.Setup(this);
		}

		void OnDisable()
		{
			Scene.Teardown();
		}

		#endregion
	}
}
