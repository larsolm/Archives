#pragma once

#include "Pargon/Types/Types.h"
#include "Pargon/Math/Math.h"
#include "Pargon/Graphics/Graphics.h"
#include "Pargon/Game/Core/GameCore.h"

#include "SuperMarioWar/Tilemap/TilemapInstance.h"

namespace SuperMarioWar
{
	class TilemapRenderer : public Pargon::Game::Core::Renderer
	{
	public:
		virtual void Reset() override;
		virtual void Render(Pargon::Game::Core::Renderable* renderable, const Pargon::Math::Matrix4x4& view, const Pargon::Math::Matrix4x4& projection) override;
		virtual void Render(Pargon::Graphics::GraphicsDevice* graphics, int width, int height) override;

	private:
		friend void Pargon::CreateMembers<TilemapRenderer>(Reflection::Type* type);

		struct ConstantBuffer
		{
			Pargon::Math::Matrix4x4 Projection;
			Pargon::Math::Matrix4x4 Transform;
			float Depth;
		};

		struct RenderInstance
		{
			TilemapInstance* Tilemap;
			ConstantBuffer Constants;
		};

		Pargon::List<RenderInstance> _instances;

		Pargon::Game::Core::GameObjectList<Pargon::Game::Core::MaterialObject> _materials;
		Pargon::Game::Core::GameObjectList<Pargon::Game::Core::TextureObject> _textures;
		Pargon::Game::Core::GameObjectList<TilemapObject> _tilemaps;

		Pargon::String _material;
	};
}

namespace Pargon
{
	template<> void CreateMembers<SuperMarioWar::TilemapRenderer>(Reflection::Type* type);
}
