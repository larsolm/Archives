#pragma once

#include "Pargon/GameCore/GameCore.h"
#include "Pargon/Types/String.h"
#include "Pargon/Types/Buffer.h"
#include "Pargon/Types/List.h"
#include "Pargon/Math/Math.h"

namespace Pargon
{
	class Renderable;

	namespace Graphics
	{
		class Texture;
		class Device;
	}
}

namespace GrungySponge
{
	class ClearField;

	class DirtRenderer : public Pargon::Renderer
	{
        friend void Pargon::CreateMembers<DirtRenderer>(Pargon::Reflection::Type* type);

	private:
		struct RenderInstance
		{
			ClearField* Dirt;
			Pargon::Matrix4x4 Transform;
		};

		Pargon::List<RenderInstance> _instances;

		Pargon::GameListReference<Pargon::BlendOptionsObject> _blendOptions;
		Pargon::GameListReference<Pargon::DepthStencilOptionsObject> _depthStencilOptions;
		Pargon::GameListReference<Pargon::RasterizeOptionsObject> _rasterizationOptions;
		Pargon::GameListReference<Pargon::MaterialObject> _materials;

		Pargon::String _blending;
		Pargon::String _depthStencil;
		Pargon::String _rasterization;
		Pargon::String _material;

		Pargon::Graphics::Geometry _geometry;

		Pargon::Graphics::Texture _redTexture;
		Pargon::Graphics::Texture _greenTexture;
		Pargon::Graphics::Texture _blueTexture;

	protected:
		virtual void Start() override;

	public:
		void SetTextures(const Pargon::String& red, const Pargon::String& green, const Pargon::String& blue);

		virtual void Reset() override;
		virtual void Render(Pargon::Renderable* renderable, const Pargon::Matrix4x4& view, const Pargon::Matrix4x4& projection) override;
		virtual void Render(Pargon::Graphics::GraphicsDevice* graphics, int width, int height) override;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::DirtRenderer>(Reflection::Type* type);
