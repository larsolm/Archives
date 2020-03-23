#include "Bonkers/Bonkers.h"

using namespace Bonkers;

void Deck::InitializeDeck()
{
	auto random = Pargon::Random();
	_tiles.Shuffle(random);
}

auto Deck::DrawTile() -> Tile*
{
	auto& tile = _tiles.Last();
	_discards.Add(std::move(tile));
	_tiles.RemoveLast();
	return _discards.Last().get();
}

template<>
void Pargon::CreateMembers<Deck>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<Deck>();

	type->AddField("Tiles", &Deck::_tiles);
}
