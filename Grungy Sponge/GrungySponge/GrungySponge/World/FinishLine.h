#pragma once

#include "Pargon/Game2D/CollisionComponent.h"
#include "Pargon/Math/Math.h"

namespace GrungySponge
{
	class FinishLine : public Pargon::CollisionComponent
	{
	public:
		Pargon::Vector2 Direction;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::FinishLine>(Reflection::Type* type);
