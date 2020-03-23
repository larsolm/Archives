#include "Bonkers/Bonkers.h"

using namespace Bonkers;

auto MoveTile::GetText() -> Pargon::String
{
	return Pargon::ToString(abs(_count));
}

void MoveTile::Process(GameManager* game, Player* player, Board* board)
{
	game->AdvancePlayer(player, _count);
}

template<>
void Pargon::CreateMembers<MoveTile>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<MoveTile>();

	type->AddField("Count", &MoveTile::_count);
}
