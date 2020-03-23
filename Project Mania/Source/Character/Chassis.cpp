#include "Pch.h"
#include "Character/Character.h"
#include "Character/Chassis.h"
#include "Data/AugmentData.h"
#include "Data/CharacterData.h"
#include "Data/ComponentData.h"
#include "Data/ChassisData.h"
#include "Data/EquipmentData.h"
#include "Data/UpgradeData.h"
#include "Data/PartData.h"
#include "Parts/Item.h"
#include "Parts/Equipment.h"
#include "Parts/Weapon.h"
#include "Parts/Upgrade.h"

void Chassis::Initialize()
{
	Skeleton = Data->Skeleton->CreateSkeleton();
	Skeleton->Data = Data->Skeleton;
	Skeleton->Character = Character;
	Skeleton->Initialize();

	for (auto& attachment : Data->Attachments)
	{
		auto part = AttachPart(attachment.Bone, *attachment.Part);

		for (auto i = 0; i < attachment.Components.Count(); i++)
			attachment.Components.Item(i)->Apply(*part, i);

		for (auto augment : attachment.Augments)
			augment->Apply(*part);
	}

	Character->Health = GetMaxHealth();
	Character->Shield = GetMaxShield();
}

void Chassis::Update(float elapsed)
{
	for (auto& equipment : Equipment)
		equipment->Update(elapsed, false, false, false);

	for (auto i = 0; i < Items.Count(); i++)
	{
		auto item = static_cast<Item*>(Items.Item(i).get());
		auto active = Character->ActiveItemSlot == i;
		auto primary = active && Character->Controller->ItemPrimary;
		auto secondary = active && Character->Controller->ItemSecondary;
		item->Update(elapsed, active, primary, secondary);
	}

	for (auto i = 0; i < Weapons.Count(); i++)
	{
		auto weapon = static_cast<Weapon*>(Weapons.Item(i).get());
		auto active = Character->ActiveWeaponSlot == i;
		auto primary = active && Character->Controller->WeaponPrimary;
		auto secondary = active && Character->Controller->WeaponSecondary;
		weapon->Update(elapsed, active, primary, secondary);
	}
}

auto Chassis::AttachPart(StringView name, PartData& data) -> Part*
{
	auto& bone = Skeleton->GetBone(name);

	assert(bone.Data->AttachmentType == data.Bone.AttachmentType);

	auto part = data.CreatePart();
	part->Data = &data;
	part->Character = Character;
	part->Bone = &bone;
	part->Initialize();

	switch (data.Bone.AttachmentType)
	{
		case AttachmentType::Equipment: return Equipment.Add(std::move(part)).get();
		case AttachmentType::Item: if (Items.IsEmpty()) Character->ActiveItemSlot = 0; return Items.Add(std::move(part)).get();
		case AttachmentType::Weapon: if (Weapons.IsEmpty()) Character->ActiveWeaponSlot = 0; return Weapons.Add(std::move(part)).get();
	}

	return nullptr;
}

auto Chassis::GetMaxHealth() const -> float
{
	auto health = Character->Data->Health;
	for (auto& part : Equipment)
	{
		auto equipment = static_cast<class Equipment*>(part.get());
		health += equipment->EquipmentData->Attributes.GetAttribute(equipment->Modifiers, EquipmentAttribute::Health);
	}

	return health;
}

auto Chassis::GetMaxShield() const -> float
{
	auto shield = 0.0f;
	for (auto& part : Equipment)
	{
		for (auto& upgrade : part->Upgrades)
			shield += upgrade->Data->Attributes.GetAttribute(upgrade->Modifiers, UpgradeAttribute::Shield);
	}

	return shield;
}

auto Chassis::GetShieldRegen() const -> float
{
	auto regen = 0.0f;
	for (auto& part : Equipment)
	{
		for (auto& upgrade : part->Upgrades)
			regen += upgrade->Data->Attributes.GetAttribute(upgrade->Modifiers, UpgradeAttribute::ShieldRegen);
	}

	return regen;
}
