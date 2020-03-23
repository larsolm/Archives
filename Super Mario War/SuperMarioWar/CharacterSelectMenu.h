#pragma once

#include "Pargon/Gui/Gui.h"
#include "Pargon/Types/Types.h"

namespace SuperMarioWar
{
	class CharacterSelectMenu : public Pargon::Gui::Menu
	{
	public:
		virtual void Activate() override;
		virtual void Update() override;

		auto IncrementSelection(int playerIndex, int amount) -> Pargon::String;
	
	private:
		Pargon::List<Pargon::String> _characters;
		Pargon::Array<int, 4> _characterIndexes;
	};
}

template<> void Pargon::CreateMembers<SuperMarioWar::CharacterSelectMenu>(Pargon::Reflection::Type* type);
