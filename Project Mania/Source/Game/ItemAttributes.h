#pragma once

#include "Game/Attributes.h"

#include <Pargon.h>

using namespace Pargon;

enum class ItemAttribute
{
	Speed,
	None
};

template<> auto EnumNames<ItemAttribute> = SetEnumNames
(
	"Speed"
);

using ItemAttributeModifiers = AttributeModifiers<ItemAttribute>;
using ItemAttributes = Attributes<ItemAttribute>;
