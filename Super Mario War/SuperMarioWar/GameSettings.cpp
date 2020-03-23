#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/GameSettings.h"

using namespace SuperMarioWar;
using namespace Pargon;

void GameSettings::IncrementPlayerType(int playerIndex, int increment)
{
	auto index = (int)_playerSettings[playerIndex].PlayerType + increment;
	if (index > (int)PlayerType::Count - 1)
		index = 0;
	else if (index < 0)
		index = 2;

	auto type = (PlayerType)index;
	_playerSettings[playerIndex].PlayerType = type;
}

template<>
void Pargon::CreateMembers<GameSettings>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<GameSettings>();

	type->AddMethod("SetPlayerType", &GameSettings::SetPlayerType);
	type->AddMethod("GetPlayerType", &GameSettings::GetPlayerType);
	type->AddMethod("IncrementPlayerType", &GameSettings::IncrementPlayerType);
	type->AddMethod("SetCharacter", &GameSettings::SetCharacter);
}

template<>
void Pargon::CreateMembers<PlayerType>(Reflection::Type* type)
{
	Serialization::SerializeAsEnum<PlayerType>();

	type->AddConstant("Human", PlayerType::Human);
	type->AddConstant("Bot", PlayerType::Bot);
	type->AddConstant("Inactive", PlayerType::Inactive);
}
