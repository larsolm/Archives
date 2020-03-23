#include "Bonkers/Bonkers.h"

using namespace Bonkers;
using namespace Pargon;

template<> void Pargon::CreateMembers<Bonkers::MainMenu>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<MainMenu>();

	type->AddField("PointsToWin", &MainMenu::_pointsToWin);
	type->AddField("NumberOfPlayers", &MainMenu::_numberOfPlayers);
	type->AddField("HandSize", &MainMenu::_handSize);
	type->AddField("Board", &MainMenu::_board);
	type->AddMethod("ModifyPlayers", &MainMenu::ModifyPlayers);
	type->AddMethod("ModifyScore", &MainMenu::ModifyScore);
	type->AddMethod("ModifyHandSize", &MainMenu::ModifyHandSize);
	type->AddMethod("AddPlayer", &MainMenu::AddPlayer);
	type->AddMethod("RemovePlayer", &MainMenu::RemovePlayer);
	type->AddMethod("CreatePlayers", &MainMenu::CreatePlayers);
}

namespace
{
	auto ClampValue(size_t value, size_t min, size_t max)
	{
		size_t result = value;

		if (value < min)
			result = min;
		else if (value > max)
			result = max;

		return result;
	}
}

void MainMenu::ModifyPlayers(int value)
{
	_numberOfPlayers = ClampValue(_numberOfPlayers + value, 2, 4);
}

void MainMenu::ModifyScore(int value)
{
	_pointsToWin = ClampValue(_pointsToWin + value, 10, 100);
}

void MainMenu::ModifyHandSize(int value)
{
	_handSize = ClampValue(_handSize + value, 1, 10);
}

void MainMenu::CreatePlayers()
{
	if (_players.Count() < _numberOfPlayers)
	{
		auto uiStack = CurrentGame()->World().GetObjects<UiStack>().ObjectNamed("UiStack");
		uiStack->Push("PlayerSelect", true);
	}
	else
	{
		Play();
	}
}

void MainMenu::Play()
{
	auto uiStack = CurrentGame()->World().GetObjects<UiStack>().ObjectNamed("UiStack");
	uiStack->Push("BoardUi", false);
	auto gameManager = CurrentGame()->World().GetObjects<GameManager>().ObjectNamed("GameManager");
	gameManager->NewGame(_players, _pointsToWin, _handSize);
}

void MainMenu::AddPlayer(String name, Color teamColor)
{
	auto player = std::make_unique<PlayerProfile>();
	player->Name = name;
	player->TeamColor = teamColor;
	_players.Add(std::move(player));
}

void MainMenu::RemovePlayer()
{
	_players.Clear();
}
