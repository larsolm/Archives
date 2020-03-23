using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/Player/Player Controller")]
	public class PlayerController : MoveController
	{
		private float _horizontal = 0.0f;
		private float _vertical = 0.0f;

		public void UpdateInput()
		{
			_horizontal = InputHelper.GetAxisValue(Player.Instance.LeftKey, Player.Instance.RightKey, Player.Instance.LeftButton, Player.Instance.RightButton, Player.Instance.HorizontalAxis);
			_vertical = InputHelper.GetAxisValue(Player.Instance.DownKey, Player.Instance.UpKey, Player.Instance.DownButton, Player.Instance.UpButton, Player.Instance.VerticalAxis);
		}

		public void ClearInput()
		{
			_horizontal = 0.0f;
			_vertical = 0.0f;
		}

		private void FixedUpdate()
		{
			UpdateMover(_horizontal, _vertical);
		}
	}
}
