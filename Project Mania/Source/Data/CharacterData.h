#pragma once

#include "Data/Data.h"

#include <Pargon.h>

class Character;
class WorldNode;
struct ChassisData;

struct CharacterData : public Data
{
	static DataMap<CharacterData> CharacterDataMap;

	ChassisData* Chassis;

	String Controller;
	String Processor;

	float Health;
	float ContactDamage;
	float Weight;
	float Aerodynamics;
	float Hops;
	float HangAmount;
	float HangTime;
	float Agility;
	float Maneuverability;
	float FallSpeed;
	float ForwardSpeed;
	float BackwardSpeed;
	float GroundHeight;
	float StandUpSpeed;
	float GravityDampening;

	Angle MaxSlopeAngle;
	Angle MaxCeilingAngle;

	List<Vector2> CollisionPolygon;

	void FromBlueprint(BlueprintReader& reader) override;
	virtual auto Create(WorldNode& node) const -> std::unique_ptr<Character>;
};
