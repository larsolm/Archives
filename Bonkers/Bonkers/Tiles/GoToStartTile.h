#pragma once

#include "Bonkers/Tile.h"

namespace Bonkers
{
	class GoToStartTile : public Tile
	{
		friend void Pargon::CreateMembers<GoToStartTile>(Pargon::Reflection::Type* type);

	public:
		virtual auto GetText() -> Pargon::String override;
		virtual void Process(GameManager* game, Player* player, Board* board) override;
	};
}

template<>
void Pargon::CreateMembers<Bonkers::GoToStartTile>(Reflection::Type* type);
