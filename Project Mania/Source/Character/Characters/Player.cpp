#include "Pch.h"
#include "Character/Characters/Player.h"
#include "Game/Game.h"

void Player::Start()
{
	GroundCharacter::Start();

	PlayerData = static_cast<struct PlayerData*>(Data);
	Game()->Rendering.Camera.Player = this;
}

void Player::Update(float elapsed)
{
	GroundCharacter::Update(elapsed);
}

void Player::OnNodeChanged(const WorldNodeData& previous)
{
	GroundCharacter::OnNodeChanged(previous);

	Game()->World.ChangeNode(previous, Node().Data);
}

void PlayerData::FromBlueprint(BlueprintReader& reader)
{
	GroundCharacterData::FromBlueprint(reader);
}

auto PlayerData::Create(WorldNode& node) const -> std::unique_ptr<Character>
{
	return std::make_unique<Player>(node);
}
