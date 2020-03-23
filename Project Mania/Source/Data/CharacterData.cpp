#include "Pch.h"
#include "Character/Character.h"
#include "Character/Characters/GroundCharacter.h"
#include "Character/Characters/Player.h"
#include "Data/CharacterData.h"
#include "Game/Game.h"

DataMap<CharacterData> CharacterData::CharacterDataMap
{
	DataMapEntry<CharacterData>{ "Character", &CreateInstance<CharacterData, CharacterData> },
	DataMapEntry<CharacterData>{ "Ground", &CreateInstance<CharacterData, GroundCharacterData> },
	DataMapEntry<CharacterData>{ "Player", &CreateInstance<CharacterData, PlayerData> }
};

void CharacterData::FromBlueprint(BlueprintReader& reader)
{
	Data::FromBlueprint(reader);

	String chassis;

	reader.ReadChild("Chassis", chassis);
	reader.ReadChild("Controller", Controller);
	reader.ReadChild("Processor", Processor);
	reader.ReadChild("Health", Health);
	reader.ReadChild("ContactDamage", ContactDamage);
	reader.ReadChild("Weight", Weight);
	reader.ReadChild("Aerodynamics", Aerodynamics);
	reader.ReadChild("Hops", Hops);
	reader.ReadChild("HangAmount", HangAmount);
	reader.ReadChild("HangTime", HangTime);
	reader.ReadChild("Agility", Agility);
	reader.ReadChild("Maneuverability", Maneuverability);
	reader.ReadChild("FallSpeed", FallSpeed);
	reader.ReadChild("ForwardSpeed", ForwardSpeed);
	reader.ReadChild("BackwardSpeed", BackwardSpeed);
	reader.ReadChild("GroundHeight", GroundHeight);
	reader.ReadChild("StandUpSpeed", StandUpSpeed);
	reader.ReadChild("GravityDampening", GravityDampening);
	reader.ReadChild("MaxSlopeAngle", MaxSlopeAngle);
	reader.ReadChild("MaxCeilingAngle", MaxCeilingAngle);
	reader.ReadChild("CollisionPolygon", CollisionPolygon);

	Chassis = Game()->Data.GetChassisData(chassis);
}

auto CharacterData::Create(WorldNode& node) const -> std::unique_ptr<Character>
{
	return std::make_unique<Character>(node);
}
