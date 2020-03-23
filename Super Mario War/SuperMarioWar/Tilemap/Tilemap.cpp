#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/Tilemap/Tilemap.h"
#include "SuperMarioWar/Tilemap/Formats/Formats.h"

#include <algorithm>

using namespace Pargon;
using namespace Pargon::Graphics;
using namespace SuperMarioWar;

auto TilemapProcessor::Load(const StringView& format, const BufferView& data, TilemapContent& content, LogWriter& log) -> bool
{
	if (Equals(format, "ptm", true))
		return Ptm::Load(data, content, log);

	log.Write("format " + format + " is not supported");
	return false;
}

void TilemapProcessor::Unload(TilemapContent& content)
{
	content.Tiles.Clear();
}

namespace
{
	struct TilemapVertex
	{
		float X, Y;
		float U, V;
	};

	struct CollisionRow
	{
		int StartX;
		int StartY;
		int EndX;
		int EndY;
		bool OneWay;
	};

	void AddCollision(List<CollisionRow>& collisions, CollisionRow collision)
	{
		if (!collision.OneWay)
		{
			for (auto& row : collisions)
			{
				if (row.StartX == collision.StartX && row.EndX == collision.EndX && row.EndY == collision.StartY && row.OneWay == collision.OneWay)
				{
					row.EndY = collision.EndY;
					return;
				}
			}
		}

		collisions.Add(collision);
	}
}

void Tilemap::Reload()
{
	_collisions.Clear();
	_data.Unload();
	auto& content = _data.Content();

	List<TilemapVertex> vertices;

	auto width = 1.0f / content.MapColumns;
	auto height = 1.0f / content.MapRows;
	auto tWidth = 1.0f / content.TextureColumns;
	auto tHeight = 1.0f / content.TextureRows;

	for (auto x = 0; x < content.MapColumns; x++)
	{
		auto sx = x * width;
		auto ex = sx + width;

		for (auto y = 0; y < content.MapRows; y++)
		{
			auto& tile = content.Tiles[y * content.MapColumns + x];

			auto u = tile.Frame < 0 ? -1 : tile.Frame % content.TextureColumns;
			auto v = tile.Frame < 0 ? -1 : tile.Frame / content.TextureColumns;

			auto sy = y * height;
			auto ey = sy + height;
			auto su = u * tWidth;
			auto sv = v * tHeight;
			auto eu = su + tWidth;
			auto ev = sv + tHeight;

			auto& vertex1 = vertices.Increment();
			vertex1.U = su;
			vertex1.V = ev;
			vertex1.X = sx;
			vertex1.Y = sy;

			auto& vertex2 = vertices.Increment();
			vertex2.U = su;
			vertex2.V = sv;
			vertex2.X = sx;
			vertex2.Y = ey;

			auto& vertex3 = vertices.Increment();
			vertex3.U = eu;
			vertex3.V = ev;
			vertex3.X = ex;
			vertex3.Y = sy;

			auto& vertex4 = vertices.Increment();
			vertex4.U = eu;
			vertex4.V = ev;
			vertex4.X = ex;
			vertex4.Y = sy;

			auto& vertex5 = vertices.Increment();
			vertex5.U = su;
			vertex5.V = sv;
			vertex5.X = sx;
			vertex5.Y = ey;

			auto& vertex6 = vertices.Increment();
			vertex6.U = eu;
			vertex6.V = sv;
			vertex6.X = ex;
			vertex6.Y = ey;
		}
	}

	auto& geometry = _geometry.Data().ManualLoad(_data.Source() + " Geometry");
	geometry.Strip = false;
	geometry.VertexCount = vertices.Count();
	geometry.VertexSize = sizeof(TilemapVertex);
	geometry.Vertices = std::make_unique<char[]>(geometry.VertexCount * geometry.VertexSize);
	std::copy(vertices.begin(), vertices.end(), reinterpret_cast<TilemapVertex*>(geometry.Vertices.get()));

	List<CollisionRow> collisions;

	for (auto row = 0; row < content.MapRows; row++)
	{
		auto startX = 0;
		auto previousCollision = CollisionType::None;

		for (auto column = 0; column < content.MapColumns; column++)
		{
			auto& tile = content.Tiles[row * content.MapColumns + column];

			if (tile.Collision == CollisionType::None || tile.Collision != previousCollision)
			{
				if (previousCollision != CollisionType::None)
					AddCollision(collisions, { startX, row, column, row + 1, previousCollision == CollisionType::OneWay });

				startX = column;
			}

			previousCollision = tile.Collision;
		}

		if (previousCollision != CollisionType::None)
			AddCollision(collisions, { startX, row, content.MapColumns, row + 1, previousCollision == CollisionType::OneWay });
	}

	auto tileWidth = 1.0f / content.MapColumns;
	auto tileHeight = 1.0f / content.MapRows;

	for (auto& collision : collisions)
		_collisions.Add({ collision.StartX * tileWidth, collision.StartY * tileHeight, (collision.EndX - collision.StartX) * tileWidth, (collision.EndY - collision.StartY) * tileHeight, collision.OneWay });
}

template<>
void Pargon::CreateMembers<Tilemap>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<Tilemap>();

	type->AddProperty("Filename", &Tilemap::Source, &Tilemap::SetFilename);
}
