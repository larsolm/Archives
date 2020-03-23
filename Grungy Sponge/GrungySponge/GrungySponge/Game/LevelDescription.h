#pragma once

#include "Pargon/Types/String.h"
#include "Pargon/Math/Math.h"
#include "GrungySponge/Sponge/SpongeData.h"

namespace GrungySponge
{
	struct LevelDescription
	{
		Pargon::String RedDirt;
		Pargon::String GreenDirt;
		Pargon::String BlueDirt;

		Pargon::String LevelName;
		Pargon::String Description;
		Pargon::String Filename;
		Pargon::Vector2 StartingPosition = Pargon::Vector2(0, 0);
		
		SpongeType SpongeType = SpongeType::Normal;

		int GoldTime = 0;
		int SilverTime = 0;
		int BronzeTime = 0;
		
		int PercentageToClear = 0;
		int ObjectsToFind = 0;
		int UncoverableObjects = 0;
		int DirtCapacity = 0;
		int Laps = 0;

		Pargon::Rectangle CameraBounds;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::LevelDescription>(Pargon::Reflection::Type* type);
