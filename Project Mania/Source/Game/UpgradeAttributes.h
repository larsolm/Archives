#pragma once

#include "Game/Attributes.h"

#include <Pargon.h>

using namespace Pargon;

enum class UpgradeAttribute
{
	Shield,
	ShieldRegen,
	None
};

template<> auto EnumNames<UpgradeAttribute> = SetEnumNames
(
	"Shield",
	"ShieldRegen"
);

using UpgradeAttributeModifiers = AttributeModifiers<UpgradeAttribute>;
using UpgradeAttributes = Attributes<UpgradeAttribute>;
