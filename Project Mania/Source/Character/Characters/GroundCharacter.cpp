#include "Pch.h"
#include "Character/Characters/GroundCharacter.h"
#include "Game/Game.h"

void GroundCharacter::Start()
{
	auto size = Collision.Size();

	ProjectedLeftGroundPosition = { ProjectedPosition.X - (size.X * 0.5f), ProjectedPosition.Y };
	ProjectedRightGroundPosition = { ProjectedPosition.X + (size.X * 0.5f), ProjectedPosition.Y };

	LeftGroundPosition = Game()->World.GetRealPosition(ProjectedLeftGroundPosition, Node().Data);
	RightGroundPosition = Game()->World.GetRealPosition(ProjectedRightGroundPosition, Node().Data);

	PreviousLeftGroundPosition = LeftGroundPosition;
	PreviousRightGroundPosition = RightGroundPosition;
}

void GroundCharacter::Update(float elapsed)
{
	PreviousLeftGroundPosition = LeftGroundPosition;
	PreviousRightGroundPosition = RightGroundPosition;

	Character::Update(elapsed);

	LeftGroundPosition = Game()->World.GetRealPosition(ProjectedLeftGroundPosition, Node().Data);
	RightGroundPosition = Game()->World.GetRealPosition(ProjectedRightGroundPosition, Node().Data);
}

void GroundCharacter::OnNodeChanged(const WorldNodeData& previous)
{
	Character::OnNodeChanged(previous);

	ProjectedLeftGroundPosition = Game()->World.GetProjectedPosition(LeftGroundPosition, Node().Data);
	ProjectedRightGroundPosition = Game()->World.GetProjectedPosition(RightGroundPosition, Node().Data);
}

void GroundCharacterData::FromBlueprint(BlueprintReader& reader)
{
	CharacterData::FromBlueprint(reader);
}

auto GroundCharacterData::Create(WorldNode& node) const -> std::unique_ptr<Character>
{
	return std::make_unique<GroundCharacter>(node);
}
