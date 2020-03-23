#pragma once

#include "Game/UpgradeAttributes.h"

#include <Pargon.h>

class Part;
struct AugmentData;
struct UpgradeData;

class Upgrade
{
public:
	Part* Part;
	UpgradeData* Data;
	UpgradeAttributeModifiers Modifiers;

	void Initialize();
	void Update(float elapsed);

protected:
	virtual void OnInitialize() {}
	virtual void OnUpdate(float elapsed) {}
};
