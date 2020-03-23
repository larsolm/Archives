#pragma once

#include "Pargon/GameUi/KeyboardMouse/KeyboardMouseMenu.h"

namespace Bonkers
{
	class PlayerSelect : public Pargon::KeyboardMouseMenu
	{
		friend void Pargon::CreateMembers<PlayerSelect>(Pargon::Reflection::Type* type);

	private:
		Pargon::String _name = "Player";
		Pargon::Color _teamColor = Pargon::Color::Blue();
	};
}

template<> void Pargon::CreateMembers<Bonkers::PlayerSelect>(Pargon::Reflection::Type* type);
