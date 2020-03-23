#pragma once

#include "Game/WeaponAbilities.h"
#include "Game/WeaponAttributes.h"
#include "Parts/Part.h"

#include <Pargon.h>

using namespace Pargon;

struct WeaponData;

class Weapon : public Part
{
public:
	WeaponData* WeaponData;
	WeaponAbilities Abilities;
	WeaponAttributeModifiers Modifiers;

protected:
	void OnInitialize() override;
	void OnUpdate(float elapsed) override;

	void StartPrimary(float elapsed) override;
	void UpdatePrimary(float elapsed) override;
	void StopPrimary(float elapsed) override;

	void StartSecondary(float elapsed) override;
	void UpdateSecondary(float elapsed) override;
	void StopSecondary(float elapsed) override;

private:
	float _rechargeTimer = 0.0;
	float _rechargeTimeout = 1.0;
	float _fireTimer = 0.0f;

	float _currentEnergy;

	void Primary();
	void Secondary();
};
