#pragma once

#include "Pargon/GameCore/GameCore.h"
#include "GrungySponge/Game/WorldDescription.h"
#include "GrungySponge/Game/LevelDescription.h"
#include "GrungySponge/World/ClearFieldManager.h"
#include "GrungySponge/Game/DirtManager.h"

namespace GrungySponge
{
    class ClearFieldManager;
    class DirtManager;
    
	class GameManager : public Pargon::GameObject
	{
	private:
		bool _levelFinished = false;

		float _clearedDirtPercent = 0;

        Pargon::GameObjectReference<ClearFieldManager> _clearFieldManager;
        Pargon::GameObjectReference<DirtManager> _dirtManager;

	public:
		bool Paused = false;
		bool IsGold = true;
		bool IsSilver = true;
		bool IsBronze = true;

		WorldDescription CurrentWorld;
		LevelDescription CurrentLevel;
		unsigned int LevelIndex = 0;

		int LapsCompleted = 0;
		int FoundObjects = 0;
		int UncoveredObjects = 0;

		int DirtCapacity = 0;
		Pargon::Array<int, 3> HeldDirt;
		
		void LoadLevel();
		void UnloadLevel();
		void RestartLevel();
		void NextLevel();
		void Calibrate();

		bool LevelFinished() const;
		float ClearedDirtPercent() const;
		float SpongeCapacity() const;

		void Update();

	private:
		void ResolveVictoryConditions();
	};
}

template<> void Pargon::CreateMembers<GrungySponge::GameManager>(Reflection::Type* type);

inline
bool GrungySponge::GameManager::LevelFinished() const
{
	return _levelFinished;
}

inline
float GrungySponge::GameManager::ClearedDirtPercent() const
{
	return _clearedDirtPercent;
}
