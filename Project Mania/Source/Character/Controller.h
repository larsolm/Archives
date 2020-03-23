#pragma once

#include "Data/Data.h"

#include <Pargon.h>

using namespace Pargon;

class Character;

class Controller
{
public:
	static DataMap<Controller> ControllerMap;

	Character* Character;

	Vector2 Aim = { 1.0f, 0.0f };

	float Horizontal = 0.0f;
	float Vertical = 0.0f;

	bool Jump = false;
	bool Interact = false;

	bool WeaponPrimary = false;
	bool WeaponSecondary = false;
	bool WeaponSwitch = false;

	bool ItemPrimary = false;
	bool ItemSecondary = false;

	bool ItemOne = false;
	bool ItemTwo = false;
	bool ItemThree = false;
	bool ItemFour = false;

	virtual void Initialize() {};
	virtual void Update(float elapsed) {};
};
