#pragma once

#include "Character/Character.h"
#include "Data/CharacterData.h"

struct GroundCharacterData : public CharacterData
{
	void FromBlueprint(BlueprintReader& reader) override;
	auto Create(WorldNode& node) const -> std::unique_ptr<Character> override;
};

class GroundCharacter: public Character
{
public:
	using Character::Character;

	Point2 ProjectedLeftGroundPosition;
	Point2 ProjectedRightGroundPosition;

	Point3 LeftGroundPosition;
	Point3 RightGroundPosition;
	Point3 PreviousLeftGroundPosition;
	Point3 PreviousRightGroundPosition;

	void Start() override;
	void Update(float elapsed) override;

protected:
	void OnNodeChanged(const WorldNodeData& previous) override;
};
