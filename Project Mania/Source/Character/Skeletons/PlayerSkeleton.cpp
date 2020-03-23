#include "Pch.h"
#include "Character/Characters/Player.h"
#include "Character/Skeletons/PlayerSkeleton.h"
#include "Character/Processors/PlayerProcessor.h"
#include "World/WorldNode.h"
#include "Game/Game.h"

#include <PargonAnimation.h>

using namespace Pargon;

void PlayerSkeleton::Initialize()
{
	Skeleton::Initialize();

	Player = static_cast<class Player*>(Character);

	LeftUpperLeg = Root.GetBone("LeftUpperLeg");
	RightUpperLeg = Root.GetBone("RightUpperLeg");
	LeftLowerLeg = Root.GetBone("LeftLowerLeg");
	RightLowerLeg = Root.GetBone("RightLowerLeg");
	LeftFoot = Root.GetBone("LeftFoot");
	RightFoot = Root.GetBone("RightFoot");

	Arm = Root.GetBone("Arm");

	LeftLeg.Add(LeftUpperLeg);
	LeftLeg.Add(LeftLowerLeg);

	RightLeg.Add(RightUpperLeg);
	RightLeg.Add(RightLowerLeg);
}

void PlayerSkeleton::Interpolate(float interpolation)
{
	Skeleton::Interpolate(interpolation);

	auto left = Pargon::Interpolate(Player->PreviousLeftGroundPosition, Player->LeftGroundPosition, interpolation);
	auto right = Pargon::Interpolate(Player->PreviousRightGroundPosition, Player->RightGroundPosition, interpolation);

	auto footOffset = 0.25f;
	if (Player->Node().Data.IsSpindle)
	{
		auto x = Cosine(Player->Node().Data.WorldAngle) * footOffset;
		auto y = Sine(Player->Node().Data.WorldAngle) * footOffset;
		left.X -= x;
		left.Y += y;
		right.X += x;
		right.Y -= y;
	}
	else
	{
		left.Z += footOffset;
		right.Z -= footOffset;
	}

	if (Player->Controller->Aim.X < 0.0f)
		std::swap(left, right);

	LeftFootPosition = { left.X, left.Y, left.Z };
	RightFootPosition = { right.X, right.Y, right.Z };
}

void PlayerSkeleton::Animate(float elapsed)
{
	auto arm = Player->Controller->Aim.GetOrientation();
	auto body = Player->Node().Data.IsSpindle ? 270.0_degrees : 0.0_radians;
	if (Player->Controller->Aim.X < 0.0f)
	{
		arm = 180_degrees - arm;
		body += 180_degrees;
	}

	auto rotation = Quaternion::CreateFromEulerAngles(body, 0.0_radians, Player->Node().Data.WorldAngle);
	auto inverse = rotation.Inverted();;

	ComputeIkChain(LeftLeg, LeftFoot, LeftFootPosition, inverse);
	ComputeIkChain(RightLeg, RightFoot, RightFootPosition, inverse);

	Root.SetRotation(rotation);
	Arm->SetRotation(Quaternion::CreateRoll(arm));

	Skeleton::Animate(elapsed);
}

void PlayerSkeletonData::FromBlueprint(BlueprintReader& reader)
{
	SkeletonData::FromBlueprint(reader);
}

auto PlayerSkeletonData::CreateSkeleton() const -> std::unique_ptr<Skeleton>
{
	return std::make_unique<PlayerSkeleton>();
}
