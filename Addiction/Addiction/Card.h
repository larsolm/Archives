#pragma once

#include "Pargon/GameCore/Component.h"

namespace Addiction
{
	enum class Suit;

	class Card : public Pargon::Component
	{
	public:
		size_t Row;
		size_t Column;
		Suit Suit;
		unsigned int Value;
		bool WasLocked;
		bool Locked;
		bool Highlighted;
	};
}

template<> void Pargon::CreateMembers<Addiction::Card>(Pargon::Reflection::Type* type);