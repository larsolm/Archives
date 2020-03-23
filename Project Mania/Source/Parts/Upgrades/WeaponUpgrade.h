#pragma once

#include "Data/UpgradeData.h"

#include <Pargon.h>

class WeaponUpgradeData : public UpgradeData
{
public:
	bool Secondary = false;

	void FromBlueprint(BlueprintReader& reader) override;
	auto CreateUpgrade() const -> std::unique_ptr<Upgrade> override;
};

class WeaponUpgrade : public Upgrade
{
public:
	WeaponUpgradeData* WeaponUpgradeData;

	void OnInitialize() override;
};
