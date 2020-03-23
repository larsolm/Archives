#pragma once

#include "Data/Data.h"

#include <Pargon.h>

struct AugmentData;
struct ComponentData;
struct PartData;
struct SkeletonData;

struct AttachmentData
{
	String Bone;
	PartData* Part;
	List<ComponentData*> Components;
	List<AugmentData*> Augments;
};

struct ChassisData : public Data
{
	SkeletonData* Skeleton;
	List<AttachmentData> Attachments;

	void FromBlueprint(BlueprintReader& reader) override;
};
