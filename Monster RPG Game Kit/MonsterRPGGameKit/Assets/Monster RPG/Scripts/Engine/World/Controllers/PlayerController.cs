using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Player))]
	[HelpURL(MonsterRpg.DocumentationUrl + "player-controller")]
	[AddComponentMenu("PiRho Soft/Controllers/Player Controller")]
	public class PlayerController : Controller
	{
		[Tooltip("The input axis to use for horizontal movement")] public string HorizontalAxis = "Horizontal";
		[Tooltip("The input axis to use for vertical movement")] public string VerticalAxis = "Vertical";
		[Tooltip("The input button to use for interacting")] public string InteractButton = "Submit";
		[Tooltip("The key to use for interacting")] public KeyCode InteractKey = KeyCode.Space;

		protected float _horizontal = 0.0f;
		protected float _vertical = 0.0f;
		protected bool _interact = false;

		void Update()
		{
			if (CanMove)
				UpdateInput();
			else
				ClearInput();
		}

		void FixedUpdate()
		{
			ProcessInput();
			_interact = false;
		}

		protected virtual bool CanMove => !WorldManager.Instance.IsFrozen && !WorldManager.Instance.IsTransitioning && !Player.Instance.IsInteracting;

		protected virtual void UpdateInput()
		{
			_horizontal = InputHelper.GetAxis(HorizontalAxis);
			_vertical = InputHelper.GetAxis(VerticalAxis);
			_interact = _interact || InputHelper.GetWasButtonPressed(InteractKey, InteractButton); // _interact should stay true until the first FixedUpdate after the button is pressed
		}

		protected virtual void ClearInput()
		{
			_horizontal = 0.0f;
			_vertical = 0.0f;
			_interact = false;
		}

		protected virtual void ProcessInput()
		{
			UpdateMover(_horizontal, _vertical);

			if (_interact)
				Player.Instance.Interact();
		}

		#region Persistence

		// these are never called since the player position data is persisted via the world loader

		internal override void Load(string saveData) { }
		internal override string Save() => "";

		#endregion
	}
}
