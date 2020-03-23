#pragma once

#include "Parts/Upgrade.h"

#include <Pargon.h>

using namespace Pargon;

struct AugmentData;
struct PartData;
struct ComponentData;
struct UpgradeData;
class Character;
class Bone;

class Part
{
public:
	Character* Character;
	PartData* Data;
	Bone* Bone;

	List<ComponentData*> Components;
	List<std::unique_ptr<Upgrade>> Upgrades;

	void Initialize();
	void Update(float elapsed, bool active, bool primary, bool secondary);
	void AddComponent(ComponentData& componentData, UpgradeData& upgradeData, int slot);

protected:
	virtual void OnInitialize() {}
	virtual void OnActivate() {}
	virtual void OnUpdate(float elapsed) {}
	virtual void OnDeactivate() {}
	virtual void OnComponentAdded() {}

	virtual void StartPrimary(float elapdes) {}
	virtual void UpdatePrimary(float elapsed) {}
	virtual void StopPrimary(float elapsed) {}

	virtual void StartSecondary(float elapsed) {}
	virtual void UpdateSecondary(float elapsed) {}
	virtual void StopSecondary(float elapsed) {}

private:
	bool _active = false;
	bool _primary = false;
	bool _secondary = false;
};
