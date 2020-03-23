#include "Addiction/Addiction.h"

using namespace Pargon;
using namespace Addiction;

template<> void Pargon::CreateMembers<Addiction::Card>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<Card>();
}
