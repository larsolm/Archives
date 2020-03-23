#pragma once

#include "Pargon/GameCore/Component.h"
#include "Pargon/Types/Color.h"
#include "Bonkers/Hand.h"

namespace Bonkers
{
	class Player : public Pargon::Component
	{
	public:
		Pargon::String Name;
		Pargon::Color TeamColor;
		size_t Score = 0;
		Hand Hand;
		size_t CurrentSpace = 0;
		bool HasLoseCard = true;
		bool Human = true;
	};
}

template<> void Pargon::CreateMembers<Bonkers::Player>(Pargon::Reflection::Type* type);
