#pragma once

#include "Data/PartData.h"
#include "Game/EquipmentAttributes.h"

#include <Pargon.h>

struct EquipmentData : public PartData
{
	EquipmentAttributes Attributes;

	void FromBlueprint(BlueprintReader& reader) override;
};
