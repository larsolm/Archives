#include "Bonkers/Bonkers.h"

using namespace Bonkers;

void BoardRenderer::Reset()
{
	_instances.Clear();
}

void BoardRenderer::Render(Pargon::Renderable* renderable, const Pargon::Matrix4x4& view, const Pargon::Matrix4x4& projection)
{
	auto board = static_cast<BoardRenderable*>(renderable);
	auto transform = Pargon::Matrix4x4::CreateIdentity();
	auto spatial = board->Entity()->GetComponent<Pargon::Spatial2D>();
	auto viewProjection = view.Multiply(projection);

	spatial->NaturalOffset.Set(0, 0);
	spatial->ApplyTransform(transform);

	transform = transform.Multiply(viewProjection);

	auto realBoard = Pargon::CurrentGame()->World().GetObjects<Board>().ObjectNamed(board->Board);
	_instances.Add(RenderInstance{ realBoard, board->Depth, transform });
}

void BoardRenderer::Render(Pargon::Graphics::GraphicsDevice* graphics, int width, int height)
{
	auto material = _materials->ObjectNamed(_material);
	auto blending = _blendOptions->ObjectNamed(_blending);
	auto testing = _depthStencilOptions->ObjectNamed(_depthStencil);
	auto rasterizing = _rasterizationOptions->ObjectNamed(_rasterization);

	graphics->SetBlendOptions(blending->Object());
	graphics->SetDepthStencilOptions(testing->Object());
	graphics->SetRasterizeOptions(rasterizing->Object());

	for (auto& instance : _instances)
	{
		auto buffer = material->Object().GetVertexConstants();
		buffer.Set("Transform", instance.Transform);
		buffer.Set("BlendColor", Pargon::Color::White());
		buffer.Set("Depth", instance.Depth);

		auto& geometry = instance.Board->Geometry();

		graphics->UseMaterial(material->Object(), buffer, Pargon::Graphics::ShaderElementBuffer());
		graphics->DrawGeometry(geometry, 0, 0);
	}
}

template<>
void Pargon::CreateMembers<BoardRenderer>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<BoardRenderer>();

	type->AddField("Material", &BoardRenderer::_material);
	type->AddField("Blending", &BoardRenderer::_blending);
	type->AddField("DepthStencil", &BoardRenderer::_depthStencil);
	type->AddField("Rasterization", &BoardRenderer::_rasterization);
}
