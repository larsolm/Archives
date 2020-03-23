#include "Pch.h"
#include "Character/Character.h"
#include "Character/Characters/GroundCharacter.h"
#include "Character/Processors/GroundProcessor.h"
#include "Data/CharacterData.h"
#include "Game/Game.h"

using namespace Pargon;
using namespace Pargon::Shapes;

void GroundProcessor::UpdateGround(float elapsed)
{
	_currentGround = nullptr;
	_leftGround = nullptr;
	_rightGround = nullptr;

	auto groundBounds = GetGroundCheckBounds();
	auto lines = Character->Node().CollisionLines.All();
	auto steepestSlope = Cosine(Character->Collision.MaxSlopeAngle);

	auto leftClosest = std::numeric_limits<float>::lowest();
	auto rightClosest = std::numeric_limits<float>::lowest();
	auto leftPosition = Point2{ Character->ProjectedPosition.X - (groundBounds.Width * 0.5f), Character->ProjectedPosition.Y - Character->Data->GroundHeight };
	auto rightPosition = Point2{ Character->ProjectedPosition.X + (groundBounds.Width * 0.5f), Character->ProjectedPosition.Y - Character->Data->GroundHeight };

	for (auto& ground : lines)
	{
		if (ground->Normal.Y >= steepestSlope && (ground->Depth == Character->Depth || (ground->Depth == CollisionDepth::Middle && Character->ForceMiddle)))
		{
			auto groundIntersection = GetIntersection(ground->Line, Segment2{ { Character->ProjectedPosition.X, Character->ProjectedPosition.Y + 0.0001f }, { Character->ProjectedPosition.X, Character->ProjectedPosition.Y - Character->Data->GroundHeight } });
			if (groundIntersection.Type != Intersection2Type::None)
			{
				auto groundHeight = groundIntersection.Point1.Y;
				auto desiredHeight = groundHeight + Character->Data->GroundHeight;
				if (IsLessThanOrCloseTo(Character->ProjectedPosition.Y, desiredHeight))
				{
					Character->EnvironmentVelocity.X = 0.0f;

					if (Character->EnvironmentVelocity.Y < 0.0f)
					{
						auto dampening = Character->Data->GravityDampening * elapsed;
						if (Character->EnvironmentVelocity.Y + dampening < 0.0f)
							Character->EnvironmentVelocity.Y += dampening;
						else
							Character->EnvironmentVelocity.Y = 0.0f;
					}

					auto acceleration = Character->Data->StandUpSpeed * elapsed;
					if (Character->ProjectedPosition.Y + acceleration < desiredHeight)
						Character->ProjectedPosition.Y += acceleration;
					else
						Character->ProjectedPosition.Y = desiredHeight;

					_currentGround = ground.get();
				}
			}

			auto intersection = GetIntersection(ground->Line, groundBounds);
			if (intersection.Type != Intersection2Type::None)
			{
				auto leftDifference = groundBounds.Left() - ground->Line.Start.X;
				if (IsGreaterThanOrCloseTo(leftDifference, 0.0f))
				{
					auto value = ground->Line.Start.Y + (leftDifference * ground->Slope);
					if (value > leftClosest)
					{
						_leftGround = ground.get();

						leftClosest = value;
						leftPosition = { groundBounds.Left(), value };
					}
				}

				auto rightDifference = ground->Line.End.X - groundBounds.Right();
				if (IsGreaterThanOrCloseTo(rightDifference, 0.0f))
				{
					auto value = ground->Line.End.Y - (rightDifference * ground->Slope);
					if (value > rightClosest)
					{
						_rightGround = ground.get();
						
						rightClosest = value;
						rightPosition = { groundBounds.Right(), value };
					}
				}
			}
		}
	}

	auto character = static_cast<GroundCharacter*>(Character);
	if (_leftGround != nullptr || _rightGround != nullptr)
	{
		character->ProjectedLeftGroundPosition = _leftGround == nullptr ? _rightGround->Line.Start : leftPosition;
		character->ProjectedRightGroundPosition = _rightGround == nullptr ? _leftGround->Line.End : rightPosition;
	}
	else
	{
		character->ProjectedLeftGroundPosition = leftPosition;
		character->ProjectedRightGroundPosition = rightPosition;
	}
}

auto GroundProcessor::OnGround() const -> bool
{
	return !Character->Collision.Grounds.IsEmpty() || _leftGround != nullptr || _rightGround != nullptr;
}

auto GroundProcessor::GetGroundCheckBounds() const -> Rectangle
{
	auto collisionSize = Character->Collision.Size();
	auto slope = Vector2::CreateFromDirection(Character->Data->MaxSlopeAngle);
	auto height = collisionSize.X * (slope.Y / slope.X);
	return { Character->ProjectedPosition.X - (collisionSize.X * 0.5f), Character->ProjectedPosition.Y - Character->Data->GroundHeight - (height * 0.5f), collisionSize.X, height + Character->Data->GroundHeight};
}

auto GroundProcessor::GetSlope() -> float
{
	if (_leftGround != nullptr || _rightGround != nullptr)
	{
		auto character = static_cast<GroundCharacter*>(Character);
		auto difference = character->ProjectedRightGroundPosition - character->ProjectedLeftGroundPosition;
		return IsCloseTo(difference.X, 0.0f) ? 0.0f : difference.Y / difference.X;
	}

	return 0.0f;
}
