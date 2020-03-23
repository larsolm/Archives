#pragma once

#include "SuperMarioWar/Tilemap/Tilemap.h"

#include "Pargon/Types/Types.h"
#include "Pargon/Game/Core/GameCore.h"

namespace SuperMarioWar
{
	class TilemapInstance : public Pargon::Game::Core::Renderable
	{
	public:
		Pargon::String Tilemap;
		Pargon::String Texture;
		float Depth = 1.0f;
		Pargon::Graphics::TextureFilterMethod TextureFiltering;
		Pargon::Graphics::TextureAddressMode TextureAddressing;

	private:
		friend class TilemapRenderer;
		friend class CollisionManager;

		TilemapObject* _tilemap;
		Pargon::Game::Core::TextureObject* _texture;
	};
}

namespace Pargon
{
	template<> void CreateMembers<SuperMarioWar::TilemapInstance>(Reflection::Type* type);
}
