#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/Tilemap/Tilemap.h"
#include "SuperMarioWar/Tilemap/Formats/PtmFormat.h"

using namespace Pargon;
using namespace SuperMarioWar;

auto Ptm::Load(const BufferView& buffer, TilemapContent& content, LogWriter& log) -> bool
{
	Blueprint blueprint;
	if (!Serialization::Pon::Load({ buffer.CharacterData(), buffer.Size() }, blueprint, log))
		return false;

	FromBlueprint(blueprint.GetChild("TextureRows"), content.TextureRows);
	FromBlueprint(blueprint.GetChild("TextureColumns"), content.TextureColumns);
	
	auto tiles = blueprint.GetChild("Tiles");

	content.MapRows = tiles.Children().Count();
	content.MapColumns = tiles.GetChild(0).Children().Count();

	auto rows = tiles.Children();
	rows.Reverse();

	for (auto& row : rows)
	{
		for (auto& column : row.Children())
			content.Tiles.Increment().Frame = static_cast<int>(column.IntegerValue());
	}

	List<int> solids;
	for (auto& solid : blueprint.GetChild("Solid").Children())
		solids.Add(static_cast<int>(solid.IntegerValue()));

	List<int> oneWays;
	for (auto& oneWay : blueprint.GetChild("OneWay").Children())
		oneWays.Add(static_cast<int>(oneWay.IntegerValue()));

	for (auto& tile : content.Tiles)
	{
		if (solids.Contains(tile.Frame))
			tile.Collision = CollisionType::Solid;
		else if (oneWays.Contains(tile.Frame))
			tile.Collision = CollisionType::OneWay;
	}

	return true;
}
