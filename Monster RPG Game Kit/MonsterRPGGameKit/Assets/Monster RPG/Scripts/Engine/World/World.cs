using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class MapLayerList : SerializedList<string> { }

	[HelpURL(MonsterRpg.DocumentationUrl + "world")]
	[CreateAssetMenu(fileName = nameof(World), menuName = "PiRho Soft/World", order = 300)]
	public class World : ScriptableObject
	{
		[Tooltip("The scene that contains the World Manager for this World")]
		[SceneReference("World Main", nameof(SetupWorldScene))]
		public SceneReference MainScene = new SceneReference();

		[Tooltip("Persistant scenes that should be loaded alongside the main scene")]
		[SceneReference("Ui Scene", nameof(SetupUiScene))]
		public List<SceneReference> UiScenes = new List<SceneReference>();

		[Tooltip("The default transition to play when the player enters a zone trigger with no transition set")]
		public Transition DefaultZoneTransition = null;

		[Tooltip("The default transition to play when the player spwans (including from a save)")]
		public Transition DefaultSpawnTransition = null;

		[Tooltip("The background music to play when the current zone does not have any")]
		public AudioClip BackgroundMusic;

		[Tooltip("Defines the variables available on the World")]
		[VariableAvailabilities(VariableDefinition.NotSaved, VariableDefinition.Saved)]
		public VariableSchema WorldSchema = new VariableSchema();

		[Tooltip("Defines the variables available on the Player")]
		[VariableAvailabilities(VariableDefinition.NotSaved, VariableDefinition.Saved)]
		public VariableSchema PlayerSchema = new VariableSchema();

		[Tooltip("Defines the variables available on Npcs")]
		[VariableAvailabilities(VariableDefinition.NotSaved, VariableDefinition.Saved)]
		public VariableSchema NpcSchema = new VariableSchema();

		[Tooltip("The layers available to Zones that are enabled and disabled based on the layer of the Zone the Player is currently in")]
		[ListDisplay]
		public MapLayerList MapLayers = new MapLayerList();

		[Tooltip("The zones in this world")]
		public List<Zone> Zones = new List<Zone>();

		#region Scene Maintenance

		void OnEnable()
		{
			MainScene.Setup(this);

			foreach (var scene in UiScenes)
				scene.Setup(this);
		}

		void OnDisable()
		{
			MainScene.Teardown();

			foreach (var scene in UiScenes)
				scene.Teardown();
		}

		#endregion

		#region Zone Lookup

		public Zone GetZoneByName(string name)
		{
			foreach (var zone in Zones)
			{
				if (zone.name == name)
					return zone;
			}

			return null;
		}

		public Zone GetZoneBySceneIndex(int index)
		{
			foreach (var zone in Zones)
			{
				if (zone.Scene.Index == index)
					return zone;
			}

			return null;
		}

		#endregion

		#region Editor Support

		private void SetupWorldScene()
		{
			var gameObject = new GameObject("World Manager");
			var worldManager = gameObject.AddComponent<WorldManager>();
			gameObject.AddComponent<AudioManager>();
			gameObject.AddComponent<InstructionManager>();
			gameObject.AddComponent<InterfaceManager>();

			worldManager.World = this;
		}

		private static void SetupUiScene()
		{
			var uiLayer = LayerMask.NameToLayer("UI");

			var cameraObject = new GameObject("Camera");
			cameraObject.layer = uiLayer;

			var camera = cameraObject.AddComponent<Camera>();
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.cullingMask = 1 << uiLayer;
			camera.orthographic = true;
			camera.depth = 1;
			camera.allowMSAA = false;

			var transition = cameraObject.AddComponent<TransitionRenderer>();

			var canvasObject = new GameObject("Canvas");
			canvasObject.layer = uiLayer;

			var canvas = canvasObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.worldCamera = camera;

			canvasObject.AddComponent<GraphicRaycaster>();
			canvasObject.AddComponent<Interface>();

			var eventSystemObject = new GameObject("EventSystem");

			eventSystemObject.AddComponent<EventSystem>();
			eventSystemObject.AddComponent<StandaloneInputModule>();
		}

		#endregion
	}
}
