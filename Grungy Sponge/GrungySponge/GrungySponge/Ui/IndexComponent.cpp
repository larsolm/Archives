#include "GrungySponge/GrungySponge.h"

using namespace GrungySponge;
using namespace Pargon;

template<> void Pargon::CreateMembers<GrungySponge::IndexComponent>(Pargon::Reflection::Type* type)
{
	Serialization::SerializeAsClass<IndexComponent>();

	type->AddField("Index", &IndexComponent::Index);
}
