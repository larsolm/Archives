#pragma once

#include "Bonkers/Tile.h"

namespace Bonkers
{
	class GoToScoreTile : public Tile
	{
		friend void Pargon::CreateMembers<GoToScoreTile>(Pargon::Reflection::Type* type);

	public:
		virtual auto GetText() -> Pargon::String override;
		virtual void Process(GameManager* game, Player* player, Board* board) override;
	};
}

template<>
void Pargon::CreateMembers<Bonkers::GoToScoreTile>(Reflection::Type* type);
