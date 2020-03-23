#pragma once

#include "Data/Data.h"

#include <Pargon.h>

class Part;
class Equipment;
class Item;
class Weapon;

struct ComponentData : public Data
{
	Map<int, int> EquipmentUpgrades;
	Map<int, int> ItemUpgrades;
	Map<int, int> WeaponUpgrades;

	auto CanApplyToEquipment(Equipment& item, int slot) const -> bool;
	auto CanApplyToItem(Item& item, int slot) const -> bool;
	auto CanApplyToWeapon(Weapon& item, int slot) const -> bool;

	void Apply(Part& part, int slot);
	void ApplyToEquipment(Equipment& item, int slot);
	void ApplyToItem(Item& item, int slot);
	void ApplyToWeapon(Weapon& item, int slot);

	void FromBlueprint(BlueprintReader& reader) override;
};
