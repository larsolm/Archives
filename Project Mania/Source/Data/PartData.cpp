#include "Pch.h"
#include "Data/PartData.h"
#include "Game/Game.h"
#include "Parts/Equipment.h"
#include "Parts/Item.h"
#include "Parts/Weapon.h"

void PartData::FromBlueprint(BlueprintReader& reader)
{
	Data::FromBlueprint(reader);

	reader.ReadChild("ComponentSlots", ComponentSlots);
}

auto PartData::CreatePart() const -> std::unique_ptr<Part>
{
	switch (Bone.AttachmentType)
	{
		case AttachmentType::Equipment: return std::make_unique<Equipment>();
		case AttachmentType::Item: return std::make_unique<Item>();
		case AttachmentType::Weapon: return std::make_unique<Weapon>();
	}

	return nullptr;
}
