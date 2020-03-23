#pragma once

#include "Bonkers/Tile.h"

namespace Bonkers
{
	enum class SpaceType
	{
		Empty,
		Tiled,
		Score,
		Lose
	};

	class Space
	{
	public:
		auto Type() const -> SpaceType;
		auto CurrentTile() const -> Tile*;

		void MakeScore();
		void MakeLose();
		void PlaceTile(Tile* tile);
		void ClearTile();

		auto Position() const -> const Pargon::Vector2&;
		void SetPosition(const Pargon::Vector2& position);

		auto Rotation() const -> float;
		void SetRotation(float rotation);

		auto Hit() const -> bool;
		void SetHit(bool hit);

	private:
		SpaceType _type = SpaceType::Empty;
		Tile* _tile;

		Pargon::Vector2 _position;
		float _rotation = 0.0f;

		bool _hit = false;
	};
}
