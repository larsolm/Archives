#pragma once

#include "Pargon/Types/Types.h"
#include "Pargon/GameCore/GameCore.h"

namespace Bonkers
{
	class Tile;

	class Deck : public Pargon::GameObject
	{
		friend void Pargon::CreateMembers<Deck>(Pargon::Reflection::Type* type);

	public:
		void InitializeDeck();

		auto DrawTile() -> Tile*;

	private:
		Pargon::List<std::unique_ptr<Tile>> _tiles;
		Pargon::List<std::unique_ptr<Tile>> _discards;
	};
}

template<>
void Pargon::CreateMembers<Bonkers::Deck>(Reflection::Type* type);
