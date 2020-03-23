#pragma once

#include "Pargon/GameUi/Touch/TouchMenu.h"
#include "Pargon/Types/List.h"
#include "Pargon/Math/Math.h"
#include "GrungySponge/Game/WorldDescription.h"

namespace Pargon
{
	class Ensemble;
}

namespace GrungySponge
{
	class WorldList : public Pargon::TouchMenu
	{
	private:
		Pargon::List<WorldDescription> _worldList;
		Pargon::List<Pargon::Ensemble*> _createdObjects;

	public:
		Pargon::String WorldTemplate;
		
		Pargon::Vector2 Position = Pargon::Vector2(0, 150);
		float Spacing = 10;

		void SetWorld(unsigned int index);
	
	protected:
		virtual void Start() override;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::WorldList>(Reflection::Type* type);
