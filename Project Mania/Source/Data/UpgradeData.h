#pragma once

#include "Data/Data.h"
#include "Game/UpgradeAttributes.h"
#include "Game/EquipmentAbilities.h"
#include "Game/ItemAbilities.h"
#include "Game/WeaponAbilities.h"
#include "Parts/Upgrade.h"

#include <Pargon.h>

struct UpgradeData : public Data
{
	static DataMap<UpgradeData> UpgradeMap;

	UpgradeAttributes Attributes;

	EquipmentAbilities EquipmentAbilities;
	ItemAbilities ItemAbilities;
	WeaponAbilities WeaponAbilities;

	void FromBlueprint(BlueprintReader& reader) override;
	virtual auto CreateUpgrade() const -> std::unique_ptr<Upgrade>;
};
