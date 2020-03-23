using System;
using System.Collections;
using System.Collections.Generic;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PiRhoSoft.MonsterRpgEngine
{
	[Serializable]
	public class TrainerReferenceList : SerializedList<VariableReference> { }

	[CreateAssetMenu(fileName = nameof(BattleGraph), menuName = "PiRho Soft/Graphs/Battle", order = 201)]
	[HelpURL(MonsterRpg.DocumentationUrl + "battle-graph")]
	public class BattleGraph : InstructionGraph
	{
		private const string _trainerMissingError = "(BBGTM) Failed to run Battle '{0}': trainer {1} has not been set";
		private const string _trainerInvalidError = "(BBGTI) Failed to run Battle '{0}': trainer {1} is not an ITrainer";
		private const string _invalidUiSceneError = "(BBGIUS) Failed to run Battle '{0}': the UiScene '{1}' could not be loaded";
		private const string _missingInterfaceError = "(BBGMI) Failed to run Battle '{0}': the UiScene '{1}' does not contain a BattleInterface";

		[Tooltip("The node to run when the battle is first initiated but not loaded")] public InstructionGraphNode Enter = null;
		[Tooltip("The node to run when the battle has finished loading")] public InstructionGraphNode Start = null;
		[Tooltip("The node to run when the battle is running")] public InstructionGraphNode Process = null;
		[Tooltip("The node to run when the battle is finished ")] public InstructionGraphNode Finish = null;
		[Tooltip("The node to run when the battle has finished unloading")] public InstructionGraphNode Exit = null;

		[SceneReference("Battle Ui", nameof(CreateUiScene))]
		[Tooltip("The scene containing the Interface for this Battle")]
		public SceneReference UiScene = new SceneReference();

		[Tooltip("The trainers that will participate in the battle")]
		[ListDisplay(EmptyText = "Add trainers that will participate in the battle", AllowCollapse = false)]
		public TrainerReferenceList Trainers = new TrainerReferenceList();

		private BattleContext _context;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			for (var i = 0; i < Trainers.Count; i++)
			{
				var trainer = Trainers[i];

				if (InstructionStore.IsInput(trainer))
					inputs.Add(VariableDefinition.Create("Trainer" + i, VariableType.Store));
			}

			base.GetInputs(inputs);
		}

		protected override IEnumerator Run(InstructionStore variables)
		{
			var trainers = new List<ITrainer>(Trainers.Count);

			for (var i = 0; i < Trainers.Count; i++)
			{
				var variable = Trainers[i].GetValue(variables);

				if (variable.Type == VariableType.Empty)
					Debug.LogErrorFormat(this, _trainerMissingError, name, Trainers[i]);
				else if (!variable.TryGetStore(out var store) || !(store is ITrainer trainer))
					Debug.LogErrorFormat(this, _trainerInvalidError, name, Trainers[i]);
				else
					trainers.Add(trainer);
			}

			if (trainers.Count == Trainers.Count)
				yield return Run(variables, trainers);
		}

		protected virtual IEnumerator Run(InstructionStore variables, IList<ITrainer> trainers)
		{
			yield return Run(variables, Enter, nameof(Enter));

			yield return Setup(variables, trainers);

			yield return Run(variables, Start, nameof(Start));
			yield return Run(variables, Process, nameof(Process));
			yield return Run(variables, Finish, nameof(Finish));

			yield return Teardown(variables);

			yield return Run(variables, Exit, nameof(Exit));
		}

		public virtual IEnumerator Setup(InstructionStore variables, IList<ITrainer> trainers)
		{
			yield return LoadScenes();

			_context = new BattleContext(trainers);

			variables.Global.AddVariable("Battle", VariableValue.Create(_context));
			variables.Context.Stores.Add("Battle", _context);
		}

		public virtual IEnumerator Teardown(InstructionStore variables)
		{
			yield return UnloadScenes();
			yield return Resources.UnloadUnusedAssets();

			variables.Context.Stores.Remove("Battle");
			variables.Global.RemoveVariable("Battle");

			_context = null;

			GC.Collect();
		}

		#region Scenes

		protected override void OnEnable()
		{
			base.OnEnable();
			UiScene.Setup(this);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			UiScene.Teardown();
		}

		protected IEnumerator LoadScenes()
		{
			if (UiScene.IsAssigned)
			{
				var loader = SceneManager.LoadSceneAsync(UiScene.Path, LoadSceneMode.Additive);

				if (loader != null)
				{
					while (!loader.isDone)
						yield return null;
				}

				if (!UiScene.IsLoaded)
				{
					Debug.LogErrorFormat(this, _invalidUiSceneError, UiScene.Path);
					yield return UnloadScenes();
				}
			}
		}

		protected IEnumerator UnloadScenes()
		{
			if (UiScene.IsAssigned)
			{
				var unloader = SceneManager.UnloadSceneAsync(UiScene.Scene);

				if (unloader != null)
				{
					while (!unloader.isDone)
						yield return null;
				}
			}
		}

		#endregion

		#region Editor Support

		private static void CreateUiScene()
		{
			var uiLayer = LayerMask.NameToLayer("UI");

			var cameraObject = new GameObject("Camera");
			cameraObject.transform.position = new Vector3(0, 0, -10);
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
			canvasObject.AddComponent<BattleInterface>();

			var eventSystemObject = new GameObject("EventSystem");

			eventSystemObject.AddComponent<EventSystem>();
			eventSystemObject.AddComponent<StandaloneInputModule>();
		}

		#endregion
	}
}
