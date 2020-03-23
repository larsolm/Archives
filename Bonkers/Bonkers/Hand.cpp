#include "Bonkers/Bonkers.h"

using namespace Bonkers;

void Hand::AddTile(Tile* tile)
{
	_tiles.Add(tile);
}

auto Hand::GetTile(size_t index) const -> Tile*
{
	return _tiles[index];
}

void Hand::RemoveTile(size_t index)
{
	_tiles.RemoveAt(index);
}

auto Hand::TileCount() -> size_t
{
	return _tiles.Count();
}
