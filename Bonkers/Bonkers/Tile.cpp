#include "Bonkers/Bonkers.h"

using namespace Bonkers;

auto Tile::Icon() const -> const Pargon::String&
{
	return _icon;
}

template<>
void Pargon::CreateMembers<Tile>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<Tile>();

	type->AddField("Icon", &Tile::_icon);
}
