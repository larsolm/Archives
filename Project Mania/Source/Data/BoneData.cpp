#include "Pch.h"
#include "Data/BoneData.h"

auto BoneData::GetBone(StringView name) -> BoneData*
{
	for (auto& child : Children)
	{
		if (child.Name == name)
			return &child;

		auto found = child.GetBone(name);
		if (found != nullptr)
			return found;
	}

	return nullptr;
}

void BoneData::FromBlueprint(BlueprintReader& reader)
{
	reader.ReadChild("Name", Name);
	reader.ReadChild("AttachmentType", AttachmentType);
	reader.ReadChild("Children", Children);
	reader.ReadChild("Position", Position);
	reader.ReadChild("Size", Size);
	reader.ReadChild("Offset", Offset);
	reader.ReadChild("Rotation", Rotation);
	reader.ReadChild("Constraints", Constraints);
	reader.ReadChild("JointDampening", JointDamping);
}

void JointConstraints::FromBlueprint(BlueprintReader& reader)
{
	reader.ReadChild("AllowYaw", AllowYaw);
	reader.ReadChild("AllowPitch", AllowPitch);
	reader.ReadChild("AllowRoll", AllowRoll);
	reader.ReadChild("MinimumYaw", MinimumYaw);
	reader.ReadChild("MaximumYaw", MaximumYaw);
	reader.ReadChild("MinimumPitch", MinimumPitch);
	reader.ReadChild("MaximumPitch", MaximumPitch);
	reader.ReadChild("MinimumRoll", MinimumRoll);
	reader.ReadChild("MaximumRoll", MaximumRoll);
}
