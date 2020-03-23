#pragma once

#include "Pargon/GameUi/KeyboardMouse/KeyboardMouseMenu.h"
#include "Pargon/Types/List.h"
#include "Pargon/Types/Color.h"

namespace Bonkers
{
	struct PlayerProfile
	{
		Pargon::String Name;
		Pargon::Color TeamColor;
	};

	class MainMenu : public Pargon::KeyboardMouseMenu
	{
		friend void Pargon::CreateMembers<MainMenu>(Pargon::Reflection::Type* type);

	private:
		size_t _numberOfPlayers = 2;
		size_t _pointsToWin = 20;
		size_t _handSize = 4;
		Pargon::String _board = "Standard";
		
		Pargon::List<std::unique_ptr<PlayerProfile>> _players;

		void ModifyPlayers(int value);
		void ModifyScore(int value);
		void ModifyHandSize(int value);

		void CreatePlayers();
		void Play();

	public:
		void AddPlayer(Pargon::String name, Pargon::Color teamColor);
		void RemovePlayer();
	};
}

template<> void Pargon::CreateMembers<Bonkers::MainMenu>(Pargon::Reflection::Type* type);
