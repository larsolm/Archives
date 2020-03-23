#include "GrungySponge/GrungySponge.h"

using namespace GrungySponge;
using namespace Pargon;

template<> void Pargon::CreateMembers<GrungySponge::WorldDescription>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<WorldDescription>();

	type->AddField("Name", &WorldDescription::Name);
	type->AddField("Description", &WorldDescription::Description);
}
