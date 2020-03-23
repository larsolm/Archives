#include "Pch.h"
#include "Data/AugmentData.h"
#include "Data/EquipmentData.h"
#include "Data/ItemData.h"
#include "Data/UpgradeData.h"
#include "Data/WeaponData.h"
#include "Parts/Equipment.h"
#include "Parts/Item.h"
#include "Parts/Upgrade.h"
#include "Parts/Weapon.h"

namespace
{
	template<typename Type, typename DataType, typename EnumType>
	auto CanApply(Type& type, DataType& data, EnumType augment)
	{
		return data.Attributes.CanIncrease(type.Modifiers, augment);
	}

	template<typename Type, typename DataType, typename EnumType>
	void Apply(Type& type, DataType& data, EnumType augment)
	{
		if (CanApply(type, data, augment))
			type.Modifiers.Augment(augment);
	}
}

auto AugmentData::CanApplyToEquipment(Equipment& equipment) const -> bool
{
	return CanApply(equipment, *equipment.EquipmentData, EquipmentAugment);
}

auto AugmentData::CanApplyToItem(Item& item) const -> bool
{
	return CanApply(item, *item.ItemData, ItemAugment);
}

auto AugmentData::CanApplyToUpgrade(Upgrade& upgrade) const -> bool
{
	return CanApply(upgrade, *upgrade.Data, UpgradeAugment);
}

auto AugmentData::CanApplyToWeapon(Weapon& weapon) const -> bool
{
	return CanApply(weapon, *weapon.WeaponData, WeaponAugment);
}

void AugmentData::Apply(Part& part)
{
	switch (part.Data->Bone.AttachmentType)
	{
		case AttachmentType::Equipment: ApplyToEquipment(static_cast<Equipment&>(part)); break;
		case AttachmentType::Item: ApplyToItem(static_cast<Item&>(part)); break;
		case AttachmentType::Weapon: ApplyToWeapon(static_cast<Weapon&>(part)); break;
		case AttachmentType::None: break;
	}
}

void AugmentData::ApplyToEquipment(Equipment& equipment)
{
	::Apply(equipment, *equipment.EquipmentData, EquipmentAugment);
}

void AugmentData::ApplyToItem(Item& item)
{
	::Apply(item, *item.ItemData, ItemAugment);
}

void AugmentData::ApplyToUpgrade(Upgrade& upgrade)
{
	::Apply(upgrade, *upgrade.Data, UpgradeAugment);
}

void AugmentData::ApplyToWeapon(Weapon& weapon)
{
	::Apply(weapon, *weapon.WeaponData, WeaponAugment);
}

void AugmentData::FromBlueprint(BlueprintReader& reader)
{
	Data::FromBlueprint(reader);

	reader.ReadChild("EquipmentAugment", EquipmentAugment);
	reader.ReadChild("ItemAugment", ItemAugment);
	reader.ReadChild("UpgradeAugment", UpgradeAugment);
	reader.ReadChild("WeaponsAugment", WeaponAugment);
}
