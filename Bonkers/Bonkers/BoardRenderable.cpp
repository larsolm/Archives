#include "Bonkers/Bonkers.h"

using namespace Bonkers;

template<>
void Pargon::CreateMembers<BoardRenderable>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<BoardRenderable>();

	type->AddField("Board", &BoardRenderable::Board);
	type->AddField("Depth", &BoardRenderable::Depth);
}
