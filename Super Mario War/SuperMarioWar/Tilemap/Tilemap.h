#pragma once

#include "Pargon/Types/Types.h"
#include "Pargon/Data/Data.h"
#include "Pargon/Math/Math.h"
#include "Pargon/Graphics/Graphics.h"

namespace SuperMarioWar
{
	enum class CollisionType
	{
		None,
		Solid,
		OneWay
	};

	struct TileContent
	{
		int Frame;
		CollisionType Collision;
	};

	struct TilemapContent
	{
		int MapRows = 10;
		int MapColumns = 10;
		int TextureRows = 1;
		int TextureColumns = 1;
		Pargon::List<TileContent> Tiles;
	};

	struct TilemapProcessor
	{
		auto Load(const Pargon::StringView& format, const Pargon::BufferView& data, TilemapContent& content, Pargon::LogWriter& log) -> bool;
		void Unload(TilemapContent& content);
	};

	using TilemapData = Pargon::DataObject<TilemapContent, TilemapProcessor>;

	struct CollisionBox
	{
		float X, Y, Width, Height;
		bool OneWay;
	};

	class Tilemap
	{
	public:
		auto Source() const -> Pargon::String;
		auto Data() -> TilemapData&;

		void SetFilename(const Pargon::String& filename);

		auto Collisions() const -> const Pargon::List<CollisionBox>&;
		auto Geometry() -> Pargon::Graphics::Geometry&;

	private:
		TilemapData _data;

		Pargon::List<CollisionBox> _collisions;
		Pargon::Graphics::Geometry _geometry;

		void Reload();
	};

	using TilemapObject = Pargon::Game::Core::GameObjectWrapper<Tilemap>;
}

namespace Pargon
{
	template<> void CreateMembers<SuperMarioWar::Tilemap>(Reflection::Type* type);
}

inline
auto SuperMarioWar::Tilemap::Source() const -> Pargon::String
{
	return _data.Source();
}

inline
void SuperMarioWar::Tilemap::SetFilename(const Pargon::String& filename)
{
	_data.SetSource<Pargon::FileSource>(filename);
	Reload();
}

inline
auto SuperMarioWar::Tilemap::Data() -> TilemapData&
{
	return _data;
}

inline
auto SuperMarioWar::Tilemap::Collisions() const -> const Pargon::List<CollisionBox>&
{
	return _collisions;
}

inline
auto SuperMarioWar::Tilemap::Geometry() -> Pargon::Graphics::Geometry&
{
	return _geometry;
}
