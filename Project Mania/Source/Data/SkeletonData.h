#pragma once

#include "Data/BoneData.h"
#include "Data/Data.h"

#include <Pargon.h>

class Skeleton;

struct SkeletonData : public Data
{
	static DataMap<SkeletonData> SkeletonMap;

	BoneData Bones;

	auto GetBone(StringView name) -> BoneData&;

	void FromBlueprint(BlueprintReader& reader) override;
	virtual auto CreateSkeleton() const -> std::unique_ptr<Skeleton>;
};
