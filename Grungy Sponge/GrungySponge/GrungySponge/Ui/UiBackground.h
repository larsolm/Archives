#pragma once

#include "Pargon/GameCore/GameCore.h"
#include "GrungySponge/World/ClearFieldManager.h"
#include "GrungySponge/Game/DirtManager.h"

namespace GrungySponge
{
	class UiBackground : public Pargon::Updater
	{
	private:
		int _totalDirt = 0;

		Pargon::GameObjectReference<ClearFieldManager> _clearFieldManager;
		Pargon::GameObjectReference<DirtManager> _dirtManager;

	public:        
		int ClearPercentage = 80;

		virtual void Start() override;

		virtual void Update(float elapsed) override;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::UiBackground>(Reflection::Type* type);
