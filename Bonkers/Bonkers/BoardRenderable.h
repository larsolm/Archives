#pragma once

#include "Pargon/GameCore/GameCore.h"

namespace Bonkers
{
	class BoardRenderable : public Pargon::Renderable
	{
	public:
		Pargon::String Board;
		float Depth;
	};
}

template<>
void Pargon::CreateMembers<Bonkers::BoardRenderable>(Reflection::Type* type);
