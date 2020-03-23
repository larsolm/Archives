#include "Pch.h"
#include "Data/PartData.h"
#include "Data/UpgradeData.h"
#include "Parts/Equipment.h"
#include "Parts/Item.h"
#include "Parts/Weapon.h"
#include "Parts/Part.h"
#include "Parts/Upgrade.h"

void Upgrade::Initialize()
{
	switch (Part->Data->Bone.AttachmentType)
	{
		case AttachmentType::Equipment: static_cast<Equipment*>(Part)->Abilities.AddAbilities(Data->EquipmentAbilities); break;
		case AttachmentType::Item: static_cast<Item*>(Part)->Abilities.AddAbilities(Data->ItemAbilities); break;
		case AttachmentType::Weapon: static_cast<Weapon*>(Part)->Abilities.AddAbilities(Data->WeaponAbilities); break;
	}

	OnInitialize();
}

void Upgrade::Update(float elapsed)
{
	OnUpdate(elapsed);
}
