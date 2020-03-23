#pragma once

#include "Character/Characters/GroundCharacter.h"

class Interaction;

struct PlayerData : public GroundCharacterData
{
	void FromBlueprint(BlueprintReader& reader) override;
	auto Create(WorldNode& node) const -> std::unique_ptr<Character> override;
};

class Player : public GroundCharacter
{
public:
	using GroundCharacter::GroundCharacter;

	PlayerData* PlayerData;
	Interaction* CurrentInteraction;

	void Start() override;
	void Update(float elapsed) override;

protected:
	void OnNodeChanged(const WorldNodeData& previous) override;
};
