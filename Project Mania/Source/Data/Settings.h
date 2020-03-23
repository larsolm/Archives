#pragma once

#include <Pargon.h>

using namespace Pargon;

struct ControllerMapping
{
	bool SwapSticks = false;

	ControllerButton JumpButton = ControllerButton::A;
	ControllerButton InteractButton = ControllerButton::X;
	ControllerButton WeaponPrimaryButton = ControllerButton::RightTrigger;
	ControllerButton WeaponSecondaryButton = ControllerButton::RightBumper;
	ControllerButton WeaponSwitchButton = ControllerButton::Y;
	ControllerButton ItemPrimaryButton = ControllerButton::LeftTrigger;
	ControllerButton ItemSecondaryButton = ControllerButton::LeftBumper;
	ControllerButton ItemOneButton = ControllerButton::Up;
	ControllerButton ItemTwoButton = ControllerButton::Left;
	ControllerButton ItemThreeButton = ControllerButton::Right;
	ControllerButton ItemFourButton = ControllerButton::Down;
};

struct KeyboardMapping
{
	Key LeftKey = Key::Left;
	Key RightKey = Key::Right;
	Key UpKey = Key::Up;
	Key DownKey = Key::Down;

	Key JumpKey = Key::Space;
	Key InteractKey = Key::Enter;
	Key WeaponPrimaryKey = Key::R;
	Key WeaponSecondaryKey = Key::N;
	Key WeaponSwitchKey = Key::Tab;
	Key ItemPrimaryKey = Key::L;
	Key ItemSecondaryKey = Key::S;
	Key ItemOneKey = Key::Number8;
	Key ItemTwoKey = Key::Number4;
	Key ItemThreeKey = Key::Number6;
	Key ItemFourKey = Key::Number2;
};

struct Settings
{
	float MusicVolume = 1.0f;
	float EffectsVolume = 1.0f;

	ControllerMapping Controller;
	KeyboardMapping Keyboard;
};
