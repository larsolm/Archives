#pragma once

#include "Data/Data.h"
#include "Game/EquipmentAttributes.h"
#include "Game/ItemAttributes.h"
#include "Game/WeaponAttributes.h"
#include "Game/UpgradeAttributes.h"

#include <Pargon.h>

class Part;
class Equipment;
class Item;
class Upgrade;
class Weapon;

struct AugmentData : public Data
{
	EquipmentAttribute EquipmentAugment = EquipmentAttribute::None;
	ItemAttribute ItemAugment = ItemAttribute::None;
	UpgradeAttribute UpgradeAugment = UpgradeAttribute::None;
	WeaponAttribute WeaponAugment = WeaponAttribute::None;

	auto CanApplyToEquipment(Equipment& item) const -> bool;
	auto CanApplyToItem(Item& item) const -> bool;
	auto CanApplyToWeapon(Weapon& item) const -> bool;
	auto CanApplyToUpgrade(Upgrade& upgrade) const -> bool;

	void Apply(Part& part);
	void ApplyToEquipment(Equipment& item);
	void ApplyToItem(Item& item);
	void ApplyToUpgrade(Upgrade& upgrade);
	void ApplyToWeapon(Weapon& item);

	void FromBlueprint(BlueprintReader& reader) override;
};
