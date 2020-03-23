#include "Pch.h"
#include "Data/SkeletonData.h"
#include "Character/Skeletons/PlayerSkeleton.h"

DataMap<SkeletonData> SkeletonData::SkeletonMap
{
	DataMapEntry<SkeletonData>{ "Character", &CreateInstance<SkeletonData, SkeletonData> },
	DataMapEntry<SkeletonData>{ "Player", &CreateInstance<SkeletonData, PlayerSkeletonData> }
};

namespace
{
	auto GetDataType(const Data& data) -> StringView
	{
		if (typeid(data) == typeid(SkeletonData)) return "Character"_sv;
		if (typeid(data) == typeid(PlayerSkeletonData)) return "Player"_sv;

		return {};
	}
}

auto SkeletonData::GetBone(StringView name) -> BoneData&
{
	auto bone = Bones.GetBone(name);
	if (bone != nullptr)
		return *bone;

	return Bones;
}

void SkeletonData::FromBlueprint(BlueprintReader& reader)
{
	Data::FromBlueprint(reader);

	reader.ReadChild("Bones", Bones);
}

auto SkeletonData::CreateSkeleton() const -> std::unique_ptr<Skeleton>
{
	return std::make_unique<Skeleton>();
}
