#include "Pch.h"
#include "Data/PartData.h"
#include "Data/UpgradeData.h"
#include "Parts/Part.h"

void Part::Initialize()
{
	Components.SetCount(Data->ComponentSlots, nullptr);

	OnInitialize();
}

void Part::Update(float elapsed, bool active, bool primary, bool secondary)
{
	if (active && !_active)
		OnActivate();
	else if (_active && !active)
		OnDeactivate();
	else if (active)
		OnUpdate(elapsed);

	_active = active;

	if (active)
	{
		if (primary && !_primary)
			StartPrimary(elapsed);
		else if (!primary && _primary)
			StopPrimary(elapsed);
		else if (primary)
			UpdatePrimary(elapsed);

		if (secondary && !_secondary)
			StartSecondary(elapsed);
		else if (!secondary && _secondary)
			StopSecondary(elapsed);
		else if (secondary)
			UpdateSecondary(elapsed);

		_primary = primary;
		_secondary = secondary;
	}
	else
	{
		if (_primary)
			StopPrimary(elapsed);
		
		if (_secondary)
			StopSecondary(elapsed);

		_primary = false;
		_secondary = false;
	}

	for (auto& upgrade : Upgrades)
		upgrade->Update(elapsed);
}

void Part::AddComponent(ComponentData& componentData, UpgradeData& upgradeData, int slot)
{
	Components.SetItem(slot, &componentData);
	auto& upgrade = Upgrades.Increment(upgradeData.CreateUpgrade());
	upgrade->Part = this;
	upgrade->Data = &upgradeData;
	upgrade->Initialize();

	OnComponentAdded();
}
