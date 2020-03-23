#pragma once

#include "Character/Skeleton.h"

class PlayerSkeleton : public Skeleton
{
public:
	Bone* LeftUpperLeg;
	Bone* RightUpperLeg;
	Bone* LeftLowerLeg;
	Bone* RightLowerLeg;
	Bone* LeftFoot;
	Bone* RightFoot;

	List<Bone*> LeftLeg;
	List<Bone*> RightLeg;

	Point3 LeftFootPosition;
	Point3 RightFootPosition;

	void Initialize() override;
	void Interpolate(float interpolation) override;
	void Animate(float elapsed) override;
};

struct BipedSkeletonData : public SkeletonData
{
	void FromBlueprint(BlueprintReader& reader) override;
	auto CreateSkeleton() const -> std::unique_ptr<Skeleton> override;
};
