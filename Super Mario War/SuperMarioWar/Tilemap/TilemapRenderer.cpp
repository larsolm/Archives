#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/Tilemap/TilemapRenderer.h"

using namespace Pargon;
using namespace SuperMarioWar;

void TilemapRenderer::Reset()
{
	_instances.Clear();
}

void TilemapRenderer::Render(Game::Core::Renderable* renderable, const Math::Matrix4x4& view, const Math::Matrix4x4& projection)
{
	auto tilemap = static_cast<TilemapInstance*>(renderable);

	if (tilemap->_tilemap == nullptr)
		tilemap->_tilemap = _tilemaps->ObjectNamed(tilemap->Tilemap);

	if (tilemap->_tilemap == nullptr)
		return;

	if (tilemap->_texture == nullptr && !tilemap->Texture.IsEmpty())
		tilemap->_texture = _textures->ObjectNamed(tilemap->Texture);

	auto& instance = _instances.Increment();
	instance.Tilemap = tilemap;
	instance.Constants.Projection = view * projection;
	instance.Constants.Transform = Math::Matrix4x4::CreateIdentity();
	instance.Constants.Depth = tilemap->Depth;

	auto spatial = tilemap->Entity()->GetComponent<Pargon::Game::X2d::Spatial>();
	spatial->ApplyTransform(instance.Constants.Transform);
}

void TilemapRenderer::Render(Graphics::GraphicsDevice* graphics, int width, int height)
{
	auto material = _materials->ObjectNamed(_material);

	for (auto& instance : _instances)
	{
		auto constants = Graphics::MakeConstantBuffer(instance.Constants);

		if (instance.Tilemap->_texture != nullptr)
			graphics->UseTexture(**instance.Tilemap->_texture, { instance.Tilemap->TextureFiltering, instance.Tilemap->TextureAddressing });

		graphics->UseMaterial(**material);
		graphics->DrawGeometry((*instance.Tilemap->_tilemap)->Geometry(), constants, Graphics::ConstantBuffer(), 0, 0);
	}
}

template<>
void Pargon::CreateMembers<TilemapRenderer>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<TilemapRenderer>();

	type->AddField("Material", &TilemapRenderer::_material);
}
