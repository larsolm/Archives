#include "Bonkers/Bonkers.h"

using namespace Bonkers;

template<> void Pargon::CreateMembers<Bonkers::Player>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<Player>();
}
