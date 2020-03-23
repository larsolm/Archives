#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace GrungySponge;

template<> void Pargon::CreateMembers<GrungySponge::Pit>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<Pit>();

	type->AddField("LeftSpawnOffset", &Pit::LeftSpawnOffset);
	type->AddField("RightSpawnOffset", &Pit::RightSpawnOffset);
	type->AddField("TopSpawnOffset", &Pit::TopSpawnOffset);
	type->AddField("BottomSpawnOffset", &Pit::BottomSpawnOffset);
}
