#pragma once

#include "Character/Skeleton.h"

class Player;

class PlayerSkeleton : public Skeleton
{
public:
	Player* Player;

	Bone* LeftUpperLeg;
	Bone* RightUpperLeg;
	Bone* LeftLowerLeg;
	Bone* RightLowerLeg;
	Bone* LeftFoot;
	Bone* RightFoot;

	Bone* Arm;

	List<Bone*> LeftLeg;
	List<Bone*> RightLeg;

	Point3 LeftFootPosition;
	Point3 RightFootPosition;

	void Initialize() override;
	void Interpolate(float interpolation) override;
	void Animate(float elapsed) override;
};

struct PlayerSkeletonData : public SkeletonData
{
	void FromBlueprint(BlueprintReader& reader) override;
	auto CreateSkeleton() const -> std::unique_ptr<Skeleton> override;
};
