#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace Pargon::Graphics;
using namespace GrungySponge;

namespace
{
	struct DirtVertex
	{
		float X, Y;
		float U1, V1;
		float U2, V2;
		float U3, V3;
		float U4, V4;
	};
}

void DirtRenderer::Start()
{
}

void DirtRenderer::Reset()
{
	_instances.Clear();
}

void DirtRenderer::Render(Renderable* renderable, const Matrix4x4& view, const Matrix4x4& projection)
{
	ClearField* field = renderable->Entity()->GetComponent<ClearField>();

	Matrix4x4 transform = Matrix4x4::CreateIdentity();

	Spatial2D* spatial = field->Entity()->GetComponent<Spatial2D>();
	spatial->NaturalOffset.Set(0, 0);

	Matrix4x4 viewProjection = view.Multiply(projection);
	spatial->ApplyTransform(transform);
	transform = transform.Multiply(viewProjection);

	RenderInstance& instance = _instances.Increment();
	instance.Dirt = field;
	instance.Transform = transform;
}

void DirtRenderer::Render(GraphicsDevice* graphics, int width, int height)
{
    auto material = _materials->ObjectNamed(_material);
    auto blending = _blendOptions->ObjectNamed(_blending);
    auto testing = _depthStencilOptions->ObjectNamed(_depthStencil);
    auto rasterizing = _rasterizationOptions->ObjectNamed(_rasterization);
    
    graphics->SetBlendOptions(blending->Object());
    graphics->SetDepthStencilOptions(testing->Object());
    graphics->SetRasterizeOptions(rasterizing->Object());

	for (RenderInstance& instance : _instances)
	{
		instance.Dirt->Mask.Data().Load();

        auto vertexBuffer = material->Object().GetVertexConstants();
		vertexBuffer.Set("Transform", instance.Transform);

		float aspectRatio = static_cast<float>(instance.Dirt->Mask.Data()->Width) / static_cast<float>(instance.Dirt->Mask.Data()->Height);

		vertexBuffer.Set("TextureScale", aspectRatio);

        auto fragmentBuffer = material->Object().GetFragmentConstants();
		fragmentBuffer.Set("MaskObject", 0);
		fragmentBuffer.Set("RedTexture", 1);
		fragmentBuffer.Set("GreenTexture", 2);
		fragmentBuffer.Set("BlueTexture", 3);
        
        graphics->UseTexture(instance.Dirt->Mask, 0);
        
        if (_redTexture.Data().HasSource())
            graphics->UseTexture(_redTexture, 1);
        else
            graphics->UseTexture(instance.Dirt->Mask, 1);

        if (_greenTexture.Data().HasSource())
            graphics->UseTexture(_greenTexture, 2);
        else
            graphics->UseTexture(instance.Dirt->Mask, 2);

        if (_blueTexture.Data().HasSource())
            graphics->UseTexture(_blueTexture, 3);
        else
            graphics->UseTexture(instance.Dirt->Mask, 3);
        
        graphics->UseMaterial(material->Object(), vertexBuffer, fragmentBuffer);
        graphics->DrawGeometry(_geometry, 0, 0);
	}
}

void DirtRenderer::SetTextures(const String& red, const String& green, const String& blue)
{
	if (red != "")
	{
		_redTexture.SetFilename(red);
		_redTexture.SetWrapU(TextureWrap::Repeat);
		_redTexture.SetWrapV(TextureWrap::Repeat);
	}

	if (green != "")
	{
		_greenTexture.SetFilename(green);
		_greenTexture.SetWrapU(TextureWrap::Repeat);
		_greenTexture.SetWrapV(TextureWrap::Repeat);
	}

	if (blue != "")
	{
		_blueTexture.SetFilename(blue);
		_blueTexture.SetWrapU(TextureWrap::Repeat);
		_blueTexture.SetWrapV(TextureWrap::Repeat);
	}

	float redSize = red != "" ? (1024.0f / _redTexture.Data()->Width) : 1.0f;
	float greenSize = green != "" ? (1024.0f / _greenTexture.Data()->Width) : 1.0f;
	float blueSize = blue != "" ? (1024.0f / _blueTexture.Data()->Width) : 1.0f;
    
    DirtVertex vertices[] = {
        { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, redSize, 0.0f, greenSize, 0.0f, blueSize },
        { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f },
        { 1.0f, 0.0f, 1.0f, 1.0f, redSize, redSize, greenSize, greenSize, blueSize, blueSize },
        { 1.0f, 1.0f, 1.0f, 0.0f, redSize, 0.0f, greenSize, 0.0f, blueSize, 0.0f }
    };

    auto& content = _geometry.Data().ManualLoad("DirtRenderer::SetTextures");
    content.Vertices = std::make_unique<char[]>(sizeof(vertices));
    std::copy(vertices, vertices + 4, reinterpret_cast<DirtVertex*>(content.Vertices.get()));

	content.Strip = true;
	content.VertexCount = 4;
	content.VertexSize = sizeof(DirtVertex);
    
    _geometry.VertexLayout.Add(ShaderElement{ "Position", ShaderElementType::Vector2, ShaderElementUsage::Position });
    _geometry.VertexLayout.Add(ShaderElement{ "Mask", ShaderElementType::Vector2, ShaderElementUsage::TextureCoordinate });
    _geometry.VertexLayout.Add(ShaderElement{ "Red", ShaderElementType::Vector2, ShaderElementUsage::TextureCoordinate });
    _geometry.VertexLayout.Add(ShaderElement{ "Green", ShaderElementType::Vector2, ShaderElementUsage::TextureCoordinate });
    _geometry.VertexLayout.Add(ShaderElement{ "Blue", ShaderElementType::Vector2, ShaderElementUsage::TextureCoordinate });
}

template<>
void Pargon::CreateMembers<DirtRenderer>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<DirtRenderer>();

	type->AddField("Blending", &DirtRenderer::_blending);
	type->AddField("DepthStencil", &DirtRenderer::_depthStencil);
	type->AddField("Rasterization", &DirtRenderer::_rasterization);
	type->AddField("Material", &DirtRenderer::_material);
}
