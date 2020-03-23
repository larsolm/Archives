#pragma once

#include "Pargon/Game/Core/GameCore.h"
#include "Pargon/Types/Types.h"

#include "SuperMarioWar/GameSettings.h"
#include "SuperMarioWar/PlayerManager.h"
#include "SuperMarioWar/CollisionManager.h"
#include "SuperMarioWar/Map.h"
#include "SuperMarioWar/Player.h"

namespace SuperMarioWar
{
	class GameManager : public Pargon::Game::Core::Updater
	{
	public:
		void Start() override;
		void Exit();
		
		void CreatePlayers();
		void Update(float elapsed) override;
		void StartGame();
		void SpawnPlayer(Player* player);
		void KillPlayer(Player* victim, Player* assailant);

		auto Score(int playerIndex) -> int;

	private:
		Pargon::Random _random;
		Pargon::String _mapName;
		GameSettings _gameSettings;
		PlayerManager _playerManager;
		CollisionManager _collisionManager;
		Map* _map;

		Pargon::List<Player*> _players;
		Pargon::Game::Core::GameObjectList<Pargon::Game::Core::GuiCamera> _guiCameras;
		Pargon::Game::Core::GameObjectList<Pargon::Game::Core::SoundObject> _sounds;
		Pargon::Game::Core::GameObjectList<Map> _maps;
	};
}

inline
auto SuperMarioWar::GameManager::Score(int playerIndex) -> int
{
	return _players[playerIndex]->Score;
}

template<> void Pargon::CreateMembers<SuperMarioWar::GameManager>(Pargon::Reflection::Type* type);
