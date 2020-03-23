#include "GrungySponge/GrungySponge.h"

using namespace GrungySponge;
using namespace Pargon;

template<> void Pargon::CreateMembers<LevelDescription>(Pargon::Reflection::Type* type)
{
	Serialization::SerializeAsClass<LevelDescription>();

	type->AddField("Name", &LevelDescription::LevelName);
	type->AddField("Description", &LevelDescription::Description);
	type->AddField("RedDirt", &LevelDescription::RedDirt);
	type->AddField("GreenDirt", &LevelDescription::GreenDirt);
	type->AddField("BlueDirt", &LevelDescription::BlueDirt);
	type->AddField("Filename", &LevelDescription::Filename);
	type->AddField("GoldTime", &LevelDescription::GoldTime);
	type->AddField("SilverTime", &LevelDescription::SilverTime);
	type->AddField("BronzeTime", &LevelDescription::BronzeTime);
	type->AddField("PercentageToClear", &LevelDescription::PercentageToClear);
	type->AddField("DirtCapacity", &LevelDescription::DirtCapacity);
	type->AddField("Laps", &LevelDescription::Laps);
	type->AddField("StartingPosition", &LevelDescription::StartingPosition);
	type->AddField("CameraBounds", &LevelDescription::CameraBounds);
	type->AddField("SpongeType", &LevelDescription::SpongeType);
}
