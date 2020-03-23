#pragma once

#include "Game/Attributes.h"

#include <Pargon.h>

using namespace Pargon;

enum class WeaponAttribute
{
	RateOfFire,
	Speed,
	Weight,
	Accuracy,
	Damage,
	Energy,
	Discharge,
	Recharge,
	None
};

template<> auto EnumNames<WeaponAttribute> = SetEnumNames
(
	"RateOfFire",
	"Speed",
	"Weight",
	"Accuracy",
	"Damage",
	"Energy",
	"Discharge",
	"Recharge"
);

using WeaponAttributeModifiers = AttributeModifiers<WeaponAttribute>;
using WeaponAttributes = Attributes<WeaponAttribute>;
