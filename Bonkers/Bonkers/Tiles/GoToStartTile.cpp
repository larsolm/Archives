#include "Bonkers/Bonkers.h"

using namespace Bonkers;

auto GoToStartTile::GetText() -> Pargon::String
{
	return "Start";
}

void GoToStartTile::Process(GameManager* game, Player* player, Board* board)
{
	auto spacesToGo = board->TotalSpaces() - player->CurrentSpace;
	game->AdvancePlayer(player, static_cast<int>(spacesToGo));
}

template<>
void Pargon::CreateMembers<GoToStartTile>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<GoToStartTile>();
}
