#include "Pch.h"
#include "Character/Character.h"
#include "Character/Skeleton.h"
#include "World/WorldNode.h"
#include "Game/Game.h"

#include <PargonAnimation.h>

using namespace Pargon;

namespace
{
	// TODO: maybe these are in data so they can be done per skeleton
	constexpr auto MaxInterations = 5;
	constexpr auto PositionThreashold = 0.001f;

	auto ClampConstraint(Angle angle, Angle min, Angle max) -> Angle
	{
		if (min < max)
			return Clamp(angle, min, max);

		if (angle < min && angle > max)
		{
			auto minDiff = min - angle;
			auto maxDiff = angle - max;
			return minDiff <= maxDiff ? min : max;
		}

		return angle;
	}

	auto CheckJointConstraints(const Quaternion& input, const JointConstraints& constraints) -> Quaternion
	{
		auto angles = input.GetEulerAngles();

		angles.Yaw = constraints.AllowYaw ? ClampConstraint(angles.Yaw, constraints.MinimumYaw, constraints.MaximumYaw) : 0.0_radians;
		angles.Pitch = constraints.AllowPitch ? ClampConstraint(angles.Pitch, constraints.MinimumPitch, constraints.MaximumPitch) : 0.0_radians;
		angles.Roll = constraints.AllowRoll ? ClampConstraint(angles.Roll, constraints.MinimumRoll, constraints.MaximumRoll) : 0.0_radians;

		return Quaternion::CreateFromEulerAngles(angles.Yaw, angles.Pitch, angles.Roll);
	}
}

void Skeleton::Initialize()
{
	Root.Initialize(Character->Chassis.Data->Skeleton->Bones);

	CreateDebugModels();
}

void Skeleton::CreateDebugModels()
{
	auto size = 0.05f;

	auto polygon = Character->Collision.CollisionPolygon();
	auto previous = polygon.Last();
	for (auto current : polygon)
	{
		_models.Increment(Game()->Rendering.CreateDebugModel({ { previous.X, previous.Y }, { current.X, current.Y} }, false, 0.0_radians, 0.0f, Red));
		previous = current;
	}

	Root.CreateModel();
}

void Skeleton::Interpolate(float interpolation)
{
	auto position = Pargon::Interpolate(Character->PreviousPosition, Character->Position, interpolation);

	Root.SetPosition(position);
}

void Skeleton::Animate(float elapsed)
{
	auto position = Root.Position();
	for (auto model : _models)
		model->Transform = Matrix4x4::CreateTransform({ position.X, position.Y, position.Z }, { 1.0f, 1.0f, 1.0f }, Quaternion::CreateFromEulerAngles(Character->Node().Data.IsSpindle ? 90_degrees : 0.0_radians, 0.0_radians, Character->Node().Data.WorldAngle), {});

	Root.Animate(elapsed);
}

auto Skeleton::GetBone(StringView name) -> Bone&
{
	auto bone = Root.GetBone(name);
	if (bone != nullptr)
		return *bone;

	return Root;
}

void Skeleton::ComputeIkChain(SequenceView<Bone*> chain, Bone* end, Point3 targetPosition, Quaternion root)
{
	for (auto iterations = 0, index = chain.LastIndex(); iterations < MaxInterations; iterations++, index--)
	{
		if (index < 0)
			index = chain.LastIndex();

		auto currentLink = chain.Item(index);
		auto currentPosition = currentLink->GetGlobalPosition();
		auto endPosition = end->GetGlobalPosition();

		if ((targetPosition - endPosition).GetLengthSquared() < PositionThreashold)
			break;

		auto currentVector = endPosition - currentPosition;
		auto targetVector = targetPosition - currentPosition;

		auto currentLength = currentVector.GetLengthSquared();
		auto targetLength = targetVector.GetLengthSquared();

		if (currentLength == 0.0f || targetLength == 0.0f)
			continue;

		currentVector.Normalize();
		targetVector.Normalize();

		auto cosineAngle = targetVector.GetDotProduct(currentVector);
		if (IsCloseTo(cosineAngle, 1.0f))
			continue;

		auto crossResult = currentVector.GetCrossProduct(targetVector).Rotated(root);
		crossResult.Normalize();

		auto turnAngle = ArcCosine(cosineAngle);
		if (turnAngle > currentLink->Data->JointDamping)
			turnAngle = currentLink->Data->JointDamping;

		auto quaternion = Quaternion::CreateFromAxisAngle(crossResult, turnAngle);
		auto rotation = CheckJointConstraints(currentLink->Rotation() * quaternion, currentLink->Data->Constraints);

		currentLink->SetRotation(rotation);
	}
}
