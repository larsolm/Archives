#include "Bonkers/Bonkers.h"

using namespace Bonkers;
using namespace Pargon;

template<> void Pargon::CreateMembers<Bonkers::PlayerSelect>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<PlayerSelect>();

	type->AddField("Name", &PlayerSelect::_name);
	type->AddField("Color", &PlayerSelect::_teamColor);
}
