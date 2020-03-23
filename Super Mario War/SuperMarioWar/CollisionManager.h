#pragma once

#include "Pargon/Types/Types.h"
#include "Pargon/Game/Core/GameCore.h"
#include "SuperMarioWar/Player.h"
#include "SuperMarioWar/Tilemap/Tilemap.h"
#include "SuperMarioWar/Tilemap/TilemapInstance.h"

namespace SuperMarioWar
{
	class GameManager;

	class CollisionManager
	{
	public:
		void ResolveTilemaps(float elapsed);
		void ResolveStomps(float elapsed, GameManager* game);

	private:
		Pargon::Game::Core::GameObjectList<Player> _players;
		Pargon::Game::Core::GameObjectList<Pargon::Game::Core::SoundObject> _sounds;
		Pargon::Game::Core::GameObjectList<TilemapInstance> _tilemaps;
	};
}

