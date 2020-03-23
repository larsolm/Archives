#pragma once

#include "Pargon/GameUi/Touch/TouchMenu.h"
#include "Pargon/Types/String.h"
#include "GrungySponge/Game/LevelDescription.h"

namespace GrungySponge
{
	class LevelList : public Pargon::TouchMenu
	{
	private:
		Pargon::List<LevelDescription> _levelList;

	public:
		Pargon::String LevelTemplate;

		float Top = 150;
		float Left = -300;
		float HorizontalSpacing = 10;
		float VerticalSpacing = 10;

		void NextLevel();
		void SetLevel(unsigned int index);

	protected:
		virtual void Start() override;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::LevelList>(Reflection::Type* type);
