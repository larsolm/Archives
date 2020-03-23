#pragma once

#include <Pargon.h>

using namespace Pargon;

enum class AttachmentType
{
	Equipment,
	Item,
	Weapon,
	None
};

template<> auto EnumNames<AttachmentType> = SetEnumNames
(
	"Equipment",
	"Item",
	"Weapon",
	"None"
);

struct JointConstraints
{
	bool AllowYaw = true;
	bool AllowPitch = true;
	bool AllowRoll = true;
	Angle MinimumYaw = 0_degrees;
	Angle MaximumYaw = 0_degrees;
	Angle MinimumPitch = 0_degrees;
	Angle MaximumPitch = 0_degrees;
	Angle MinimumRoll = 0_degrees;
	Angle MaximumRoll = 0_degrees;

	void FromBlueprint(BlueprintReader& reader);
};

struct BoneData
{
	String Name;
	AttachmentType AttachmentType = AttachmentType::None;
	List<BoneData> Children;

	JointConstraints Constraints;
	Angle JointDamping = 180_degrees;

	Point3 Position;
	Vector3 Size;
	Vector3 Offset;
	Quaternion Rotation = Quaternion::CreateIdentity();

	auto GetBone(StringView name) -> BoneData*;
	
	void FromBlueprint(BlueprintReader& reader);
};
