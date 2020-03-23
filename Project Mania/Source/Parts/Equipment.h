#pragma once

#include "Game/EquipmentAbilities.h"
#include "Game/EquipmentAttributes.h"
#include "Parts/Part.h"

#include <Pargon.h>

struct EquipmentData;

using namespace Pargon;

class Equipment : public Part
{
public:
	EquipmentData* EquipmentData;
	EquipmentAbilities Abilities;
	EquipmentAttributeModifiers Modifiers;

protected:
	void OnInitialize() override;
};
