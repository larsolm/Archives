#pragma once

#include "Pargon/Game2D/CollisionComponent.h"
#include "Pargon/Math/Math.h"

namespace GrungySponge
{
	class Pit : public Pargon::CollisionComponent
	{
	public:
		Pargon::Vector2 LeftSpawnOffset;
		Pargon::Vector2 RightSpawnOffset;
		Pargon::Vector2 TopSpawnOffset;
		Pargon::Vector2 BottomSpawnOffset;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::Pit>(Reflection::Type* type);
