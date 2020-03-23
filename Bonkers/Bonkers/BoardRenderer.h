#pragma once

#include "Pargon/GameCore/GameCore.h"

namespace Bonkers
{
	class BoardRenderer : public Pargon::Renderer
	{
		friend void Pargon::CreateMembers<BoardRenderer>(Pargon::Reflection::Type* type);

	public:
		virtual void Reset();
		virtual void Render(Pargon::Renderable* renderable, const Pargon::Matrix4x4& view, const Pargon::Matrix4x4& projection);
		virtual void Render(Pargon::Graphics::GraphicsDevice* graphics, int width, int height);

	private:
		struct RenderInstance
		{
			Board* Board;
			float Depth;
			Pargon::Matrix4x4 Transform;
		};

		Pargon::List<RenderInstance> _instances;

		Pargon::GameListReference<Pargon::BlendOptionsObject> _blendOptions;
		Pargon::GameListReference<Pargon::DepthStencilOptionsObject> _depthStencilOptions;
		Pargon::GameListReference<Pargon::RasterizeOptionsObject> _rasterizationOptions;
		Pargon::GameListReference<Pargon::MaterialObject> _materials;

		Pargon::String _material;
		Pargon::String _blending;
		Pargon::String _depthStencil;
		Pargon::String _rasterization;
	};
}

template<>
void Pargon::CreateMembers<Bonkers::BoardRenderer>(Reflection::Type* type);
