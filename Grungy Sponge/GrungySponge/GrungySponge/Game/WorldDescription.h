#pragma once

#include "Pargon/Types/String.h"

namespace GrungySponge
{
	struct WorldDescription
	{
		Pargon::String Name;
		Pargon::String Description;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::WorldDescription>(Reflection::Type* type);
