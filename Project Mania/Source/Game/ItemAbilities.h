#pragma once

#include "Game/Abilities.h"

#include <Pargon.h>

using namespace Pargon;

enum class ItemAbility
{
	Hookshot
};

template<> auto EnumNames<ItemAbility> = SetEnumNames
(
	"Hookshot"
);

using ItemAbilities = Abilities<ItemAbility>;