#include "Pch.h"
#include "Parts/Upgrades/WeaponUpgrade.h"

void WeaponUpgrade::OnInitialize()
{
	WeaponUpgradeData = static_cast<class WeaponUpgradeData*>(Data);
}

void WeaponUpgradeData::FromBlueprint(BlueprintReader& reader)
{
	UpgradeData::FromBlueprint(reader);

	reader.ReadChild("ForSecondary", Secondary);
}

auto WeaponUpgradeData::CreateUpgrade() const -> std::unique_ptr<Upgrade>
{
	return std::make_unique<WeaponUpgrade>();
}
