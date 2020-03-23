#include "Pch.h"
#include "Character/Character.h"
#include "Character/Controllers/PlayerController.h"
#include "World/WorldNode.h"
#include "Game/Game.h"

using namespace Pargon;

void PlayerController::Update(float elapsed)
{
	// TODO: decide which one to do based on settings (or do both?)
	Update(Game()->ControllerState(), 0);
	//Update(Game()->KeyboardState(), Game()->MouseState());
}

void PlayerController::Update(const ControllerState& controller, int controllerIndex)
{
	auto& settings = Game()->Settings.Controller;

	auto moveStick = controller.LeftStick(controllerIndex);
	auto aimStick = controller.RightStick(controllerIndex);

	if (settings.SwapSticks)
		std::swap(moveStick, aimStick);

	if (aimStick.GetLengthSquared() >= 0.25f)
	{
		Aim = aimStick;
		if (Character->Node().Data.InvertCamera)
			Aim.X *= -1;
	}

	if (AbsoluteValue(moveStick.Y) < 0.8f)
		moveStick.Y = 0.0f;

	Horizontal = Character->Node().Data.InvertCamera ? -moveStick.X : moveStick.X;
	Vertical = moveStick.Y;
	Jump = controller.IsButtonDown(controllerIndex, settings.JumpButton);
	Interact = controller.WasButtonJustPressed(controllerIndex, settings.InteractButton);
	WeaponPrimary = controller.IsButtonDown(controllerIndex, settings.WeaponPrimaryButton);
	WeaponSecondary = controller.IsButtonDown(controllerIndex, settings.WeaponSecondaryButton);
	WeaponSwitch = controller.IsButtonDown(controllerIndex, settings.WeaponSwitchButton);
	ItemPrimary = controller.IsButtonDown(controllerIndex, settings.ItemPrimaryButton);
	ItemSecondary = controller.IsButtonDown(controllerIndex, settings.ItemSecondaryButton);
	ItemOne = controller.IsButtonDown(controllerIndex, settings.ItemOneButton);
	ItemTwo = controller.IsButtonDown(controllerIndex, settings.ItemTwoButton);
	ItemThree = controller.IsButtonDown(controllerIndex, settings.ItemThreeButton);
	ItemFour = controller.IsButtonDown(controllerIndex, settings.ItemFourButton);
}

void PlayerController::Update(const KeyboardState& keyboard, const MouseState& mouse)
{
	auto& settings = Game()->Settings.Keyboard;

	auto left = keyboard.IsKeyDown(settings.LeftKey) ? 1.0f : 0.0f;
	auto right = keyboard.IsKeyDown(settings.RightKey) ? 1.0f : 0.0f;
	auto up = keyboard.IsKeyDown(settings.LeftKey) ? 1.0f : 0.0f;
	auto down = keyboard.IsKeyDown(settings.RightKey) ? 1.0f : 0.0f;
	auto aim = right - left;

	Aim = { aim, 0.0f }; // TODO: find the mouse position relative to the player.
	Horizontal = Character->Node().Data.InvertCamera ? left - right : right - left;
	Vertical = up - down;
	Jump = keyboard.IsKeyDown(settings.JumpKey);
	Interact = keyboard.WasKeyJustPressed(settings.InteractKey);
	WeaponPrimary = keyboard.IsKeyDown(settings.WeaponPrimaryKey);
	WeaponSecondary = keyboard.IsKeyDown(settings.WeaponSecondaryKey);
	WeaponSwitch = keyboard.IsKeyDown(settings.WeaponSwitchKey);
	ItemPrimary = keyboard.IsKeyDown(settings.ItemPrimaryKey);
	ItemSecondary = keyboard.IsKeyDown(settings.ItemSecondaryKey);
	ItemOne = keyboard.IsKeyDown(settings.ItemOneKey);
	ItemTwo = keyboard.IsKeyDown(settings.ItemTwoKey);
	ItemThree = keyboard.IsKeyDown(settings.ItemThreeKey);
	ItemFour = keyboard.IsKeyDown(settings.ItemFourKey);
}
