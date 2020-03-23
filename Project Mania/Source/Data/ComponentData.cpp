#include "Pch.h"
#include "Data/ComponentData.h"
#include "Game/Game.h"
#include "Parts/Equipment.h"
#include "Parts/Item.h"
#include "Parts/Weapon.h"
#include "Parts/Upgrade.h"

namespace
{
	auto CanApply(const Map<int, int>& upgrades, SequenceView<ComponentData*> components, int componentIndex, int partIndex, int slot) -> bool
	{
		if (upgrades.GetIndex(partIndex) == Sequence::InvalidIndex)
			return false;

		for (auto i = 0; i < components.Count(); i++)
		{
			auto component = components.Item(i);
			if (component != nullptr && (i == slot || component->Index == componentIndex))
				return false;
		}

		return true;
	}

	void Apply(const Map<int, int>& upgrades, Part& part, ComponentData* component , int slot)
	{
		if (CanApply(upgrades, part.Components, component->Index, part.Data->Index, slot))
		{
			auto index = upgrades.ItemWithKey(part.Data->Index);
			auto data = Game()->Data.GetUpgradeData(index);
			part.AddComponent(*component, *data, slot);
		}
	}
}

auto ComponentData::CanApplyToEquipment(Equipment& equipment, int slot) const -> bool
{
	return CanApply(EquipmentUpgrades, equipment.Components, Index, equipment.Data->Index, slot);
}

auto ComponentData::CanApplyToItem(Item& item, int slot) const -> bool
{
	return CanApply(ItemUpgrades, item.Components, Index, item.Data->Index, slot);
}

auto ComponentData::CanApplyToWeapon(Weapon& weapon, int slot) const -> bool
{
	return CanApply(WeaponUpgrades, weapon.Components, Index, weapon.Data->Index, slot);
}

void ComponentData::Apply(Part& part, int slot)
{
	switch (part.Data->Bone.AttachmentType)
	{
		case AttachmentType::Equipment: ApplyToEquipment(static_cast<Equipment&>(part), slot); break;
		case AttachmentType::Item: ApplyToItem(static_cast<Item&>(part), slot); break;
		case AttachmentType::Weapon: ApplyToWeapon(static_cast<Weapon&>(part), slot); break;
		case AttachmentType::None: break;
	}
}

void ComponentData::ApplyToEquipment(Equipment& equipment, int slot)
{
	::Apply(EquipmentUpgrades, equipment, this, slot);
}

void ComponentData::ApplyToItem(Item& item, int slot)
{
	::Apply(ItemUpgrades, item, this, slot);
}

void ComponentData::ApplyToWeapon(Weapon& weapon, int slot)
{
	::Apply(WeaponUpgrades, weapon, this, slot);
}

void ComponentData::FromBlueprint(BlueprintReader& reader)
{
	Data::FromBlueprint(reader);

	Map<String, String> equipment;
	Map<String, String> items;
	Map<String, String> weapons;

	reader.ReadChild("EquipmentUpgrades", equipment);
	reader.ReadChild("ItemUpgrades", items);
	reader.ReadChild("WeaponUpgrades", weapons);

	for (auto i = 0; i < equipment.Count(); i++)
	{
		auto& partName = equipment.GetKey(i);
		auto& upgradeName = equipment.ItemAtIndex(i);

		auto part = Game()->Data.GetEquipmentData(partName);
		auto upgrade = Game()->Data.GetUpgradeData(upgradeName);

		EquipmentUpgrades.AddOrSet(part->Index, upgrade->Index);
	}

	for (auto i = 0; i < items.Count(); i++)
	{
		auto& partName = items.GetKey(i);
		auto& upgradeName = items.ItemAtIndex(i);

		auto part = Game()->Data.GetItemData(partName);
		auto upgrade = Game()->Data.GetUpgradeData(upgradeName);

		ItemUpgrades.AddOrSet(part->Index, upgrade->Index);
	}

	for (auto i = 0; i < weapons.Count(); i++)
	{
		auto& partName = weapons.GetKey(i);
		auto& upgradeName = weapons.ItemAtIndex(i);

		auto part = Game()->Data.GetWeaponData(partName);
		auto upgrade = Game()->Data.GetUpgradeData(upgradeName);

		WeaponUpgrades.AddOrSet(part->Index, upgrade->Index);
	}
}
