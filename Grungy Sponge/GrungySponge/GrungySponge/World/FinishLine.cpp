#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace GrungySponge;

template<> void Pargon::CreateMembers<GrungySponge::FinishLine>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<FinishLine>();

	type->AddField("Direction", &FinishLine::Direction);
}
