#include "Bonkers/Bonkers.h"

using namespace Bonkers;

auto GoToScoreTile::GetText() -> Pargon::String
{
	return "Score";
}

void GoToScoreTile::Process(GameManager* game, Player* player, Board* board)
{
	auto spacesToGo = board->GetNextScore(player->CurrentSpace);
	game->AdvancePlayer(player, spacesToGo);
}

template<>
void Pargon::CreateMembers<GoToScoreTile>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<GoToScoreTile>();
}
