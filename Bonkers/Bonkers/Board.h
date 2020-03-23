#pragma once

#include "Pargon/Types/Types.h"
#include "Pargon/GameCore/GameCore.h"

#include "Bonkers/Space.h"

namespace Bonkers
{
	class Tile;

	struct CubicBezierPathNode
	{
		Pargon::Vector2 Position;
		float Rotation;
		float Weight;
	};

	struct CubicBezierPath
	{
		Pargon::List<CubicBezierPathNode> Nodes;

		auto CalculateLength() -> float;
		auto CalculatePosition(float advance) -> Pargon::Vector2;
	};

	class Board : public Pargon::GameObject
	{
		friend void Pargon::CreateMembers<Bonkers::Board>(Pargon::Reflection::Type* type);

	public:
		auto Geometry() -> Pargon::Graphics::Geometry&;

		void InitializeBoard();
		auto GetSpacePosition(size_t index) const -> Pargon::Vector2;
		auto GetSpace(size_t index) -> Space&;
		auto TotalSpaces() const -> const size_t;
		auto GetNextScore(size_t index) const -> const size_t;

		void ClearHit();
		void PlaceTile(Tile* tile, size_t index);

	private:
		size_t _numberOfSpaces = 50;
		Pargon::List<size_t> _scoreLocations;
		Pargon::List<size_t> _loseLocations;
		CubicBezierPath _shape;

		Pargon::String _tileTemplate;

		Pargon::List<Space> _spaces;
		Pargon::Graphics::Geometry _geometry;

		void GenerateBoard();
		void GenerateGeometry();
	};
}

template<>
void Pargon::CreateMembers<Bonkers::CubicBezierPathNode>(Reflection::Type* type);

template<>
void Pargon::CreateMembers<Bonkers::CubicBezierPath>(Reflection::Type* type);

template<>
void Pargon::CreateMembers<Bonkers::Board>(Reflection::Type* type);
