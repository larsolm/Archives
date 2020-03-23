#pragma once

#include "Game/Abilities.h"

#include <Pargon.h>

using namespace Pargon;

enum class WeaponAbility
{
	Secondary,
	NoRecoil,
	Ballistic,
	Dynamic
};

template<> auto EnumNames<WeaponAbility> = SetEnumNames
(
	"Secondary",
	"NoRecoil",
	"Ballistic",
	"Dynamic"
);

using WeaponAbilities = Abilities<WeaponAbility>;