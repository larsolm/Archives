#include "Pch.h"
#include "Data/UpgradeData.h"
#include "Parts/Upgrades/WeaponUpgrade.h"

DataMap<UpgradeData> UpgradeData::UpgradeMap
{
	DataMapEntry<UpgradeData>{ "Upgrade", &CreateInstance<UpgradeData, UpgradeData> },
	DataMapEntry<UpgradeData>{ "WeaponUpgrade", &CreateInstance<UpgradeData, WeaponUpgradeData> }
};

namespace
{
	auto GetDataType(const Data& data) -> StringView
	{
		if (typeid(data) == typeid(WeaponUpgradeData)) return "WeaponUpgrade"_sv;

		return {};
	}
}

void UpgradeData::FromBlueprint(BlueprintReader& reader)
{
	Data::FromBlueprint(reader);

	reader.ReadChild("Attributes", Attributes);
}

auto UpgradeData::CreateUpgrade() const -> std::unique_ptr<Upgrade>
{
	return std::make_unique<Upgrade>();
}
