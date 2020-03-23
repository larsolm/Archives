#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/Tilemap/TilemapInstance.h"

using namespace Pargon;
using namespace SuperMarioWar;

template<>
void Pargon::CreateMembers<TilemapInstance>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<TilemapInstance>();

	type->AddField("Tilemap", &TilemapInstance::Tilemap);
	type->AddField("Texture", &TilemapInstance::Texture);
	type->AddField("Depth", &TilemapInstance::Depth);
	type->AddField("TextureFiltering", &TilemapInstance::TextureFiltering);
	type->AddField("TextureAddressing", &TilemapInstance::TextureAddressing);
}
