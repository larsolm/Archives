#pragma once

#include "Data/PartData.h"
#include "Game/WeaponAttributes.h"

#include <Pargon.h>

struct WeaponData : public PartData
{
	WeaponAttributes Attributes;

	void FromBlueprint(BlueprintReader& reader) override;
};
