#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/Map.h"
#include "SuperMarioWar/Tilemap/Tilemap.h"
#include "SuperMarioWar/Tilemap/TilemapInstance.h"

using namespace SuperMarioWar;
using namespace Pargon;

void Map::LoadMap()
{
	auto& tilemapContent = (*_tilemaps->ObjectNamed(TilemapName))->Data().Content();
	auto tilemapObject = static_cast<Game::Core::Entity*>(Globals().Game->CreateObject("TilemapTemplate", TilemapName + "Instance"));
	auto tilemapSpatial = tilemapObject->GetComponent<Game::X2d::Spatial>();
	auto tilemapInstance = tilemapObject->GetComponent<TilemapInstance>();

	tilemapInstance->Tilemap = TilemapName;
	_mapWidth = tilemapSpatial->Size.X();
	_mapHeight = tilemapSpatial->Size.Y();
	_mapColumns = tilemapContent.MapColumns;
	_mapRows = tilemapContent.MapRows;
}

auto Map::GetMapPosition(int column, int row) -> Math::Vector2
{
	Math::Vector2 position;
	auto tileWidth = _mapWidth / _mapColumns;
	auto tileHeight = _mapHeight / _mapColumns;
	position.X() = (column * tileWidth) - (_mapWidth / 2) + (tileWidth / 2);
	position.Y() = (row * tileHeight) - (_mapHeight / 2) + (tileHeight / 2);
	return position;
}

template<>
void Pargon::CreateMembers<SpawnPoint>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<SpawnPoint>();

	type->AddField("Column", &SpawnPoint::Column);
	type->AddField("Row", &SpawnPoint::Row);
}

template<>
void Pargon::CreateMembers<Map>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<Map>();

	type->AddField("SpawnPoints", &Map::SpawnPoints);
	type->AddField("TilemapName", &Map::TilemapName);
	type->AddField("MapObjects", &Map::MapObjects);
}

auto SpawnPoint::operator==(const SpawnPoint& right) const -> bool
{
	return Column == right.Column && Row == right.Row;
}

auto SpawnPoint::operator<(const SpawnPoint& right) const -> bool
{
	return Column <= right.Column && Row < right.Row;
}
