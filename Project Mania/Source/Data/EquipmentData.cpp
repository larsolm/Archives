#include "Pch.h"
#include "Data/EquipmentData.h"

void EquipmentData::FromBlueprint(BlueprintReader& reader)
{
	PartData::FromBlueprint(reader);

	Bone.AttachmentType = AttachmentType::Equipment;

	reader.ReadChild("Attributes", Attributes);
}