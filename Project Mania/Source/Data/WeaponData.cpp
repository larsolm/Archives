#include "Pch.h"
#include "Data/WeaponData.h"

void WeaponData::FromBlueprint(BlueprintReader& reader)
{
	PartData::FromBlueprint(reader);

	Bone.AttachmentType = AttachmentType::Weapon;

	reader.ReadChild("Attributes", Attributes);
}
