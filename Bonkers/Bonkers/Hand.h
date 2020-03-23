#pragma once

#include "Pargon/Types/List.h"
#include "Bonkers/Tile.h"

namespace Bonkers
{
	class Hand
	{
	public:
		void AddTile(Tile* tile);
		auto GetTile(size_t index) const -> Tile*;
		void RemoveTile(size_t index);
		auto TileCount() -> size_t;

	private:
		Pargon::List<Tile*> _tiles;
	};
}