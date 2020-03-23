#pragma once

#include "Bonkers/Tile.h"

namespace Bonkers
{
	class MoveTile : public Tile
	{
		friend void Pargon::CreateMembers<MoveTile>(Pargon::Reflection::Type* type);

	public:
		virtual auto GetText() -> Pargon::String override;
		virtual void Process(GameManager* game, Player* player, Board* board) override;
		
	private:
		int _count = 0;
	};
}

template<>
void Pargon::CreateMembers<Bonkers::MoveTile>(Reflection::Type* type);
