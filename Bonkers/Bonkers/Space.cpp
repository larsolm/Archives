#include "Bonkers/Bonkers.h"

using namespace Bonkers;

auto Space::Type() const -> SpaceType
{
	return _type;
}

auto Space::CurrentTile() const -> Tile*
{
	return _tile;
}

void Space::MakeScore()
{
	_type = SpaceType::Score;
}

void Space::MakeLose()
{
	_type = SpaceType::Lose;
}

void Space::PlaceTile(Tile* tile)
{
	_type = SpaceType::Tiled;
	_tile = tile;
}

void Space::ClearTile()
{
	_type = SpaceType::Empty;
}

auto Space::Position() const -> const Pargon::Vector2&
{
	return _position;
}

void Space::SetPosition(const Pargon::Vector2& position)
{
	_position = position;
}

auto Space::Rotation() const -> float
{
	return _rotation;
}

void Space::SetRotation(float rotation)
{
	_rotation = rotation;
}

auto Space::Hit() const -> bool
{
	return _hit;
}

void Space::SetHit(bool hit)
{
	_hit = hit;
}
