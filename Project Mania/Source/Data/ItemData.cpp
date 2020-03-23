#include "Pch.h"
#include "Data/ItemData.h"

void ItemData::FromBlueprint(BlueprintReader& reader)
{
	PartData::FromBlueprint(reader);

	Bone.AttachmentType = AttachmentType::Item;

	reader.ReadChild("Attributes", Attributes);
}
