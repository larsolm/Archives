#include "Bonkers/Bonkers.h"

using namespace Bonkers;

auto RollAgainTile::GetText() -> Pargon::String
{
	return "";
}

void RollAgainTile::Process(GameManager* game, Player* player, Board* board)
{
	game->Roll(true);
}

template<>
void Pargon::CreateMembers<RollAgainTile>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<RollAgainTile>();
}
