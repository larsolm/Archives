#pragma once

#include "Data/BoneData.h"
#include "Data/Data.h"

#include <Pargon.h>

class Part;

struct PartData : public Data
{
	BoneData Bone;

	int ComponentSlots;

	void FromBlueprint(BlueprintReader& reader) override;
	auto CreatePart() const -> std::unique_ptr<Part>;
};
