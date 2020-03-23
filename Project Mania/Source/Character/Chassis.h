#pragma once

#include "Character/Skeleton.h"
#include "Parts/Part.h"

#include <Pargon.h>

using namespace Pargon;

struct ChassisData;
class Character;
class Part;
class Skeleton;

class Chassis
{
public:
	Character* Character;
	ChassisData* Data;

	List<std::unique_ptr<Part>> Equipment;
	List<std::unique_ptr<Part>> Items;
	List<std::unique_ptr<Part>> Weapons;
	std::unique_ptr<Skeleton> Skeleton;

	void Initialize();
	void Update(float elapsed);

	auto AttachPart(StringView name, PartData& part) -> Part*;
	auto GetMaxHealth() const -> float;
	auto GetMaxShield() const -> float;
	auto GetShieldRegen() const -> float;
};
