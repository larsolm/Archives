#include "Pch.h"
#include "Character/Character.h"
#include "Data/WeaponData.h"
#include "Game/Duration.h"
#include "Game/Game.h"
#include "Parts/Weapon.h"
#include "Parts/Upgrades/WeaponUpgrade.h"

void Weapon::OnInitialize()
{
	WeaponData = static_cast<struct WeaponData*>(Data);

	_currentEnergy = WeaponData->Attributes.GetAttribute(Modifiers, WeaponAttribute::Energy);
}

void Weapon::OnUpdate(float elapsed)
{
	_fireTimer += elapsed;
	_rechargeTimer += elapsed;

	if (_rechargeTimer > _rechargeTimeout)
	{
		auto recharge = WeaponData->Attributes.GetAttribute(Modifiers, WeaponAttribute::Recharge);
		auto maxEnergy = WeaponData->Attributes.GetAttribute(Modifiers, WeaponAttribute::Energy);

		_currentEnergy += recharge * elapsed;
		_currentEnergy = Minimum(_currentEnergy, maxEnergy);
	}
}

void Weapon::StartPrimary(float elapsed)
{
	auto rateOfFire = WeaponData->Attributes.GetAttribute(Modifiers, WeaponAttribute::RateOfFire);
	if (_fireTimer >= 1 / rateOfFire)
		Primary();
}

void Weapon::UpdatePrimary(float elapsed)
{
	auto rateOfFire = WeaponData->Attributes.GetAttribute(Modifiers, WeaponAttribute::RateOfFire);
	if (_fireTimer >= 1 / rateOfFire)
		Primary();
}

void Weapon::StopPrimary(float elapsed)
{
}

void Weapon::StartSecondary(float elapsed)
{
	if (Abilities.HasAbility(WeaponAbility::Secondary))
	{
		auto rateOfFire = WeaponData->Attributes.GetAttribute(Modifiers, WeaponAttribute::RateOfFire);
		if (_fireTimer >= 1 / rateOfFire)
			Secondary();
	}
}

void Weapon::UpdateSecondary(float elapsed)
{
	if (Abilities.HasAbility(WeaponAbility::Secondary))
	{
		auto rateOfFire = WeaponData->Attributes.GetAttribute(Modifiers, WeaponAttribute::RateOfFire);
		if (_fireTimer >= 1 / rateOfFire)
			Secondary();
	}
}

void Weapon::StopSecondary(float elapsed)
{
	if (Abilities.HasAbility(WeaponAbility::Secondary))
	{
	}
}

void Weapon::Primary()
{
	auto discharge = WeaponData->Attributes.GetAttribute(Modifiers, WeaponAttribute::Discharge);
	if (_currentEnergy >= discharge)
	{
		_fireTimer = 0.0f;
		_rechargeTimer = 0.0f;
		_currentEnergy -= discharge;

		auto rotation = Angle(ArcTangent(Character->Controller->Aim.X, Character->Controller->Aim.Y));
		auto& projectile = Game()->World.CreateProjectile(Character->Node(), Character->ProjectedPosition, rotation);
		projectile.Owner = Character;
		projectile.ApplyAttributes(Abilities, WeaponData->Attributes, Modifiers);
	}
}

void Weapon::Secondary()
{
	auto discharge = WeaponData->Attributes.GetAttribute(Modifiers, WeaponAttribute::Discharge);
	if (_currentEnergy >= discharge)
	{
		_fireTimer = 0.0f;
		_rechargeTimer = 0.0f;
		_currentEnergy -= discharge;

		auto rotation = Angle(ArcTangent(Character->Controller->Aim.X, Character->Controller->Aim.Y));
		auto& projectile = Game()->World.CreateProjectile(Character->Node(), Character->ProjectedPosition, rotation);
		projectile.Owner = Character;
		projectile.ApplyAttributes(Abilities, WeaponData->Attributes, Modifiers);
	}
}
