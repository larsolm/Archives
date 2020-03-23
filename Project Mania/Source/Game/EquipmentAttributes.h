#pragma once

#include "Game/Attributes.h"

#include <Pargon.h>

using namespace Pargon;

enum class EquipmentAttribute
{
	Health,
	None
};

template<> auto EnumNames<EquipmentAttribute> = SetEnumNames
(
	"Health"
);

using EquipmentAttributeModifiers = AttributeModifiers<EquipmentAttribute>;
using EquipmentAttributes = Attributes<EquipmentAttribute>;
