#pragma once

#include "Pargon/Game/Core/GameCore.h"
#include "Pargon/Types/Types.h"
#include "Pargon/Math/Math.h"
#include "SuperMarioWar/Tilemap/Tilemap.h"

namespace SuperMarioWar
{
	struct SpawnPoint
	{
		int Column = 0;
		int Row = 0;

		auto operator==(const SpawnPoint& right) const -> bool;
		auto operator<(const SpawnPoint& right) const -> bool;
	};

	class Map : public Pargon::Game::Core::GameObject
	{
	public:
		Pargon::List<SpawnPoint> SpawnPoints;
		Pargon::String TilemapName;
		Pargon::SparseArray<Pargon::String> MapObjects;

		void LoadMap();
		auto GetMapPosition(int column, int row) -> Pargon::Math::Vector2;

	private:
		float _mapWidth = 0;
		float _mapHeight = 0;
		int _mapRows = 0;
		int _mapColumns = 0;

		Pargon::Game::Core::GameObjectList<TilemapObject> _tilemaps;
	};
}

template<> void Pargon::CreateMembers<SuperMarioWar::SpawnPoint>(Pargon::Reflection::Type* type);
template<> void Pargon::CreateMembers<SuperMarioWar::Map>(Pargon::Reflection::Type* type);

