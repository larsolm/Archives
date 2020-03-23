#pragma once

#include "Data/PartData.h"
#include "Game/ItemAttributes.h"

#include <Pargon.h>

struct ItemData : public PartData
{
	ItemAttributes Attributes;

	void FromBlueprint(BlueprintReader& reader) override;
};
