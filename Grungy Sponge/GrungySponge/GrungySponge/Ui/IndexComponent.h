#pragma once

#include "Pargon/GameCore/GameCore.h"

namespace GrungySponge
{
	struct IndexComponent : public Pargon::Component
	{
		unsigned int Index = 0;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::IndexComponent>(Pargon::Reflection::Type* type);
