#pragma once

#include "Pargon/Game/Core/GameCore.h"
#include "Pargon/Game/X2d/GameX2d.h"
#include "SuperMarioWar/Player.h"

namespace SuperMarioWar
{
	class PlayerManager
	{
	public:
		void StartGame();
		void UpdatePlayers(float elapsed);

	private:
		void ProcessInput(Player* player);
		void MovePlayer(Player* player, float elapsed);
		void AnimatePlayer(Player* player, float elapsed);
		
		float _worldMinX;
		float _worldMaxX;
		float _worldMinY;
		float _worldMaxY;

		Pargon::Game::Core::GameObjectList<Player> _players;
		Pargon::Game::Core::GameObjectList<Pargon::Game::Core::SoundObject> _sounds;
		Pargon::Game::Core::GameObjectList<Pargon::Game::X2d::OrthographicCamera> _cameras;
	};
}
