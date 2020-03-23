#pragma once

#include "Pargon/GameCore/GameCore.h"
#include "Pargon/Math/Math.h"

namespace GrungySponge
{
	enum SpongeType
	{
		Normal,
		Jumping,
		Speed,
		Burst,
		Flip
	};

	class SpongeData : public Pargon::Component
	{
	public:
		Pargon::Vector2 MinimumSize = Pargon::Vector2(100, 100);
		
		float Maneuverability = 1;
		float MaxAngularVelocity = 10;
		float Acceleration = 1;
		float MaxSpeed = 100;
		float Friction = 0.97f;
		float AngularFriction = 0.97f;

		SpongeType CurrentType = SpongeType::Normal;
		float JumpHeight = 10;
		float JumpScale = 1;
		float Gravity = 10;
		
		float BurstRecharge = 3;
		float SpeedRecharge = 3;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::SpongeData>(Pargon::Reflection::Type* type);
template<> void Pargon::CreateMembers<GrungySponge::SpongeType>(Pargon::Reflection::Type* type);
