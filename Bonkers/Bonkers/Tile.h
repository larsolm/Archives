#pragma once

#include "Pargon/GameCore/GameCore.h"

namespace Bonkers
{
	class Tile
	{
		friend void Pargon::CreateMembers<Tile>(Pargon::Reflection::Type* type);

	public:
		virtual ~Tile() {}

		auto Icon() const-> const Pargon::String&;
		virtual auto GetText() -> Pargon::String { return ""; };
		virtual void Process(GameManager* game, Player* player, Board* board) {};

	private:
		Pargon::String _icon;
	};
}

template<>
void Pargon::CreateMembers<Bonkers::Tile>(Reflection::Type* type);
