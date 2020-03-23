#pragma once

#include "Pargon/GameCore/GameCore.h"
#include "Pargon/Types/Array.h"
#include "Pargon/Types/String.h"
#include "GrungySponge/Game/LevelDescription.h"
#include "GrungySponge/World/ClearFieldManager.h"

namespace GrungySponge
{
	struct DirtDescription
	{
		Pargon::String TextureFilename;
		float CleanContributionFactor = 1.0f;
		float SpongeContributionFactor = 1.0f;
	};

	struct DirtData
	{
		DirtDescription* Description;
		float ContributionPercent;
		int TotalDirt;
		int ClearedDirt;
	};

	class DirtManager : public Pargon::GameObject
	{
	private:
		Pargon::Array<DirtData, 3> _dirtData;

		int _totalDirt = 0;
		int _levelDirtCapacity = 0;

	public:
		void LoadLevel(const LevelDescription& level, ClearFieldManager* clearFieldManager);
		int CalculateDirtCapacity(const Pargon::Array<int, 3>& heldDirt);
		float CalculateClearedDirtPercent(ClearFieldManager* clearFieldManager);

		int TotalDirt();
		int LevelDirtCapacity();

		Pargon::Dictionary<std::unique_ptr<DirtDescription>> Dirts;
		DirtDescription* GetDirt(const Pargon::String& dirt);
		const Pargon::String& GetTexture(const Pargon::String& dirt);
	};
}

template<> void Pargon::CreateMembers<GrungySponge::DirtManager>(Reflection::Type* type);
template<> void Pargon::CreateMembers<GrungySponge::DirtDescription>(Reflection::Type* type);

inline
int GrungySponge::DirtManager::TotalDirt()
{
	return _totalDirt;
}

inline
int GrungySponge::DirtManager::LevelDirtCapacity()
{
	return _levelDirtCapacity;
}
