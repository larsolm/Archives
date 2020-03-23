#pragma once

#include "Game/Abilities.h"

#include <Pargon.h>

using namespace Pargon;

enum class EquipmentAbility
{
	DoubleJump
};

template<> auto EnumNames<EquipmentAbility> = SetEnumNames
(
	"DoubleJump"
);

using EquipmentAbilities = Abilities<EquipmentAbility>;