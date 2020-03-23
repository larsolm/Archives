#pragma once

#include "Game/ItemAbilities.h"
#include "Game/ItemAttributes.h"
#include "Parts/Part.h"

#include <Pargon.h>

using namespace Pargon;

struct ItemData;

class Item : public Part
{
public:
	ItemData* ItemData;
	ItemAbilities Abilities;
	ItemAttributeModifiers Modifiers;

protected:
	void OnInitialize() override;
};
