using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public struct ButtonState
	{
		public bool Pressed;
		public bool Held;
		public bool Released;
	}

	public static class InputHelper
	{
		public static ButtonState GetButtonState(KeyCode key, string button)
		{
			return new ButtonState
			{
				Pressed = GetWasButtonPressed(key, button),
				Held = GetButtonDown(key, button),
				Released = GetWasButtonReleased(key, button)
			};
		}

		public static bool GetButtonDown(KeyCode key, string button)
		{
			return Input.GetKey(key) || (!string.IsNullOrEmpty(button) && Input.GetButton(button));
		}
		
		public static bool GetWasButtonPressed(KeyCode key, string button)
		{
			return Input.GetKeyDown(key) || (!string.IsNullOrEmpty(button) && Input.GetButtonDown(button));
		}
		
		public static bool GetWasButtonReleased(KeyCode key, string button)
		{
			return Input.GetKeyUp(key) || (!string.IsNullOrEmpty(button) && Input.GetButtonUp(button));
		}

		public static float GetAxisValue(KeyCode negativeKey, KeyCode positiveKey, string negativeButton, string positiveButton, string axis)
		{
			var stick = string.IsNullOrEmpty(axis) ? 0.0f : Input.GetAxis(axis);
			var positive = Input.GetKey(positiveKey) || (!string.IsNullOrEmpty(positiveButton) && Input.GetButton(positiveButton));
			var negative = Input.GetKey(negativeKey) || (!string.IsNullOrEmpty(negativeButton) && Input.GetButton(negativeButton));
			var keys = (positive ? 1.0f : 0.0f) - (negative ? 1.0f : 0.0f);
			return Mathf.Abs(keys) > Mathf.Abs(stick) ? keys : stick;
		}
	}
}
