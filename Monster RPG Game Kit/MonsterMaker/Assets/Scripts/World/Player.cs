using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[RequireComponent(typeof(PlayerController))]
	[RequireComponent(typeof(Mover))]
	[AddComponentMenu("Monster Maker/Player/Player")]
	public class Player : Singleton<Player>
	{
		[Tooltip("The instructions to run when the pause button is pressed.")] public Instruction OnPause;
		[Space]
		[Tooltip("Variables that will be passed to instruction contexts when this player is used.")] public VariableStore Variables;
		
		[Header("Input Mapping")]
		[Tooltip("The Input Axis used for horizontal movement as specified in \"Edit -> Project Settings -> Input\"")] public string HorizontalAxis = string.Empty;
		[Tooltip("The Input Axis used for vertical movement as specified in \"Edit -> Project Settings -> Input\"")] public string VerticalAxis = string.Empty;
		[Tooltip("The Input Axis used for the left button as specified in \"Edit -> Project Settings -> Input\"")] public string LeftButton = string.Empty;
		[Tooltip("The Input Axis used for the right button as specified in \"Edit -> Project Settings -> Input\"")] public string RightButton = string.Empty;
		[Tooltip("The Input Axis used for the up button as specified in \"Edit -> Project Settings -> Input\"")] public string UpButton = string.Empty;
		[Tooltip("The Input Axis used for the down button as specified in \"Edit -> Project Settings -> Input\"")] public string DownButton = string.Empty;
		[Tooltip("The Input Axis used for the interact button as specified in \"Edit -> Project Settings -> Input\"")] public string InteractButton = string.Empty;
		[Tooltip("The Input Axis used for the pause button as specified in \"Edit -> Project Settings -> Input\"")] public string PauseButton = string.Empty;
		[Tooltip("The Input Axis used for the cancel button as specified in \"Edit -> Project Settings -> Input\"")] public string CancelButton = string.Empty;

		[Space]
		[Tooltip("The KeyCode used for the left button")] public KeyCode LeftKey = KeyCode.LeftArrow;
		[Tooltip("The KeyCode used for the right button")] public KeyCode RightKey = KeyCode.RightArrow;
		[Tooltip("The KeyCode used for the up buttton")] public KeyCode UpKey = KeyCode.UpArrow;
		[Tooltip("The KeyCode used for the down button")] public KeyCode DownKey = KeyCode.DownArrow;
		[Tooltip("The KeyCode used for the interact button")] public KeyCode InteractKey = KeyCode.Space;
		[Tooltip("The KeyCode used for the pause button")] public KeyCode PauseKey = KeyCode.Return;
		[Tooltip("The KeyCode used for the cancel button")] public KeyCode CancelKey = KeyCode.Escape;

		public PlayerController Controller { get; private set; }
		public Mover Mover { get; private set; }

		public ButtonState Interact { get; protected set; }
		public ButtonState Cancel { get; protected set; }
		public ButtonState Pause { get; protected set; }
		public ButtonState Left { get; protected set; }
		public ButtonState Right { get; protected set; }
		public ButtonState Up { get; protected set; }
		public ButtonState Down { get; protected set; }

		public void Initialize(GameManager.PlayerData data)
		{
			Mover.OnTileChanged += OnTileChanged;
			WorldManager.Instance.OnZoneEntered += OnZoneEntered;
			WorldManager.Instance.OnZoneLeft += OnZoneLeft;
		}

		protected override void OnAwake()
		{
			Controller = GetComponent<PlayerController>();
			Mover = GetComponent<Mover>();

			_interactionContext = new WorldInstructionContext(gameObject, "PlayerInteractions", Variables);
			_pauseContext = new WorldInstructionContext(gameObject, "PlayerPause", Variables);
		}

		protected virtual void Update()
		{
			Interact = InputHelper.GetButtonState(InteractKey, InteractButton);
			Cancel = InputHelper.GetButtonState(CancelKey, CancelButton);
			Pause = InputHelper.GetButtonState(PauseKey, PauseButton);
			Left = InputHelper.GetButtonState(LeftKey, LeftButton);
			Right = InputHelper.GetButtonState(RightKey, LeftButton);
			Up = InputHelper.GetButtonState(UpKey, UpButton);
			Down = InputHelper.GetButtonState(DownKey, DownButton);

			Controller.ClearInput();

			if (_interactionContext.IsRunning)
				ProcessInteractionInput();
			else if (_pauseContext.IsRunning)
				ProcessPauseInput();
			else
				ProcessWorldInput();
		}

		protected virtual void OnZoneEntered(ZoneData zone)
		{
			Mover.SetCurrentZone(zone);
			Mover.OccupyCurrentTiles();
		}

		protected virtual void OnZoneLeft(ZoneData zone)
		{
			Mover.UnoccupyCurrentTiles();
			Mover.SetCurrentZone(null);
		}

		protected virtual void OnTileChanged(Vector2Int previous, Vector2Int current)
		{
			if (Mover.CurrentZone != null)
			{
				var previousTile = Mover.CurrentZone.Tilemap.GetTile(previous);
				var currentTile = Mover.CurrentZone.Tilemap.GetTile(current);

				if (previousTile != null)
				{
					if (previousTile.HasZoneTrigger && !previousTile.IsSameZoneAs(currentTile))
						previousTile.Zone.Leave();
				}

				if (currentTile != null)
				{
					if (currentTile.HasZoneTrigger && !currentTile.IsSameZoneAs(previousTile))
						Mover.DelayForFrame = currentTile.Zone.Enter();

					if (currentTile.HasEncounter && currentTile.Encounter != null)
						Mover.DelayForFrame = currentTile.Encounter.Enter();

					if (currentTile.HasInstructions && currentTile.Instructions != null)
					{
						Mover.DelayForFrame = true; // TODO: this works but will also cause a pause for instructions that don't want it...
						InstructionManager.Instance.StartCoroutine(_interactionContext.Execute(currentTile.Instructions, this));
					}
				}
			}
		}

		private RaycastHit2D[] _interactions = new RaycastHit2D[8];
		private InstructionContext _interactionContext;
		private InstructionContext _pauseContext;

		private void ProcessWorldInput()
		{
			Controller.UpdateInput();

			if (Mover.CanInteract)
			{
				if (Interact.Pressed)
				{
					var position = new Vector2(transform.position.x, transform.position.y);
					var count = Physics2D.LinecastNonAlloc(position, position + Mover.MoveDirection, _interactions);
					for (var i = 0; i < count; i++)
					{
						var interaction = _interactions[i].collider.GetComponent<Interaction>();
						if (interaction)
						{
							var mover = interaction.GetComponent<Mover>();
							if (!mover || mover.CanInteract)
								interaction.Interact(_interactionContext);
						}
					}
				}
				else if (Pause.Pressed)
				{
					if (OnPause != null)
						_pauseContext.Run(OnPause, this);
				}
			}
		}

		private void ProcessPauseInput()
		{
			_pauseContext.ProcessInput();
		}

		private void ProcessInteractionInput()
		{
			_interactionContext.ProcessInput();
		}
	}
}
