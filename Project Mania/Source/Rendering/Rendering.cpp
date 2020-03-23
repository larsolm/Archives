#include "Pch.h"
#include "Game/Game.h"
#include "Rendering/Rendering.h"

#include <PargonDrawing.h>

using namespace Pargon;
using namespace Pargon::Drawing;
using namespace Pargon::Shapes;

Rendering::Rendering(GraphicsDevice& graphics) :
	_graphics(graphics)
{
}

void Rendering::Setup()
{
	auto depthBuffer = _graphics.CreateTexture(GraphicsStorage::GpuOnly);
	depthBuffer->Reset({ 1920, 1080 }, TextureFormat::DepthStencilTarget24_8, "World Depth Buffer");
	depthBuffer->Unlock();

	_texturedModelMaterial = _graphics.CreateMaterial(GraphicsStorage::TransferredToGpu);
	_texturedModelMaterial->Reset("Assets/Materials/TexturedModel.hlsl");
	_texturedModelMaterial->CullMode = MaterialCullMode::CounterClockwise;
	_texturedModelMaterial->FillMode = MaterialFillMode::Fill;
	_texturedModelMaterial->DepthOptions = StandardDepthTesting;
	_texturedModelMaterial->SampleOptions.Add({ SampleFilterMethod::Anisotropic, SampleAddressMode::Clamp });
	_texturedModelMaterial->VertexLayout.Add({ ShaderElementType::Vector3, ShaderElementUsage::Position });
	_texturedModelMaterial->VertexLayout.Add({ ShaderElementType::Vector3, ShaderElementUsage::Other });
	_texturedModelMaterial->VertexLayout.Add({ ShaderElementType::Vector3, ShaderElementUsage::Other });
	_texturedModelMaterial->VertexLayout.Add({ ShaderElementType::Vector3, ShaderElementUsage::Other });
	_texturedModelMaterial->VertexLayout.Add({ ShaderElementType::Vector2, ShaderElementUsage::Coordinate });
	_texturedModelMaterial->Output.Add({ DisabledBlending, DisabledBlending });
	_texturedModelMaterial->Unlock();

	_coloredModelMaterial = _graphics.CreateMaterial(GraphicsStorage::TransferredToGpu);
	_coloredModelMaterial->Reset("Assets/Materials/ColoredModel.hlsl");
	_coloredModelMaterial->CullMode = MaterialCullMode::None;
	_coloredModelMaterial->FillMode = MaterialFillMode::Fill;
	_coloredModelMaterial->DepthOptions = StandardDepthTesting;
	_coloredModelMaterial->VertexLayout.Add({ ShaderElementType::Vector3, ShaderElementUsage::Position });
	_coloredModelMaterial->VertexLayout.Add({ ShaderElementType::Color, ShaderElementUsage::Color });
	_coloredModelMaterial->Output.Add({ DisabledBlending, DisabledBlending });
	_coloredModelMaterial->Unlock();

	_diffuse = _graphics.CreateTexture(GraphicsStorage::TransferredToGpu);
	_diffuse->Reset(ApplicationDirectory().GetFile("Assets/Diffuse.png"));
	_diffuse->Unlock();

	_normal = _graphics.CreateTexture(GraphicsStorage::TransferredToGpu);
	_normal->Reset(ApplicationDirectory().GetFile("Assets/Normal.png"));
	_normal->Unlock();

	_occlusion = _graphics.CreateTexture(GraphicsStorage::TransferredToGpu);
	_occlusion->Reset(ApplicationDirectory().GetFile("Assets/Occlusion.png"));
	_occlusion->Unlock();

	_roughness = _graphics.CreateTexture(GraphicsStorage::TransferredToGpu);
	_roughness->Reset(ApplicationDirectory().GetFile("Assets/Roughness.png"));
	_roughness->Unlock();

	_metallic = _graphics.CreateTexture(GraphicsStorage::TransferredToGpu);
	_metallic->Reset(ApplicationDirectory().GetFile("Assets/Metallic.png"));
	_metallic->Unlock();

	Camera.Initialize();

	auto renderPass = Game()->Scene.AddRenderPass(Camera, AllObjects);
	renderPass->ClearColor = true;
	renderPass->ClearDepthStencil = true;
	renderPass->DepthStencilTarget = depthBuffer->Id();
}

void Rendering::Interpolate(float interpolation)
{
	Camera.Interpolate(interpolation);

	for (auto& character : Game()->World.Characters.All())
		character->Interpolate(interpolation);

	for (auto& projectile : Game()->World.Projectiles.All())
		projectile->Interpolate(interpolation);
}

void Rendering::Animate(Time elapsed)
{
	auto time = static_cast<float>(elapsed.InSeconds());

	Camera.Animate(elapsed);

	for (auto& character : Game()->World.Characters.All())
		character->Animate(time);

	for (auto& projectile : Game()->World.Projectiles.All())
		projectile->Animate(time);
}

void Rendering::Render()
{
	Game()->Scene.Draw();
}

auto Rendering::CreateModel() -> Model*
{
	// Eventually pass in all the things that you want here like actual mesh, textures, etc.

	auto vertices = Game()->Graphics.CreateGeometry(GraphicsStorage::TransferredToGpu);
	auto indices = Game()->Graphics.CreateGeometry(GraphicsStorage::TransferredToGpu);
	vertices->Reset(GeometryTopology::TriangleList, 0);
	indices->Reset(GeometryTopology::IndexList, 0);

	auto builder = GeometryBuilder(*_texturedModelMaterial, *vertices, *indices);
	builder.DrawLightedCuboid({ 1.0f, 1.0f, 1.0f });

	auto model = Game()->Scene.AddModel();
	model->Tag = AllObjects;
	model->Surface.Material = _texturedModelMaterial->Id();
	model->Surface.Textures.Item(Surface::AlbedoTexture) = _diffuse->Id();
	model->Surface.Textures.Item(Surface::NormalMap) = _normal->Id();
	model->Surface.Textures.Item(2) = _occlusion->Id();
	model->Surface.Textures.Item(3) = _roughness->Id();
	model->Mesh.SetVertexBuffer(vertices->Retreive<TexturedModelVertex>(builder.VertexStart(), builder.VertexCount()));
	model->Mesh.SetIndexBuffer(indices->Retreive<uint16_t>(builder.IndexStart(), builder.IndexCount()));

	vertices->Unlock();
	indices->Unlock();

	return model;
}

auto Rendering::CreateDebugModel(Segment2 line, bool spindle, Angle worldAngle, float z, Color color) -> Model*
{
	// This will eventually be gone and just standard debug drawing.

	auto vertices = _graphics.CreateGeometry(GraphicsStorage::TransferredToGpu);
	auto indices = _graphics.CreateGeometry(GraphicsStorage::TransferredToGpu); 
	vertices->Reset(GeometryTopology::TriangleList, 4);
	indices->Reset(GeometryTopology::IndexList, 6);

	auto builder = GeometryBuilder(*_coloredModelMaterial, *vertices, *indices);
	builder.DrawSegment(line, LineStyle(color, 0.1f));

	auto model = Game()->Scene.AddModel();
	model->Tag = AllObjects;
	model->Surface.Material = _coloredModelMaterial->Id();
	model->Mesh.SetVertexBuffer(vertices->Retreive<ColoredModelVertex>(builder.VertexStart(), builder.VertexCount()));
	model->Mesh.SetIndexBuffer(indices->Retreive<uint16_t>(builder.IndexStart(), builder.IndexCount()));
	model->Transform = Matrix4x4::CreateTransform({ 0.0f, 0.0f, z }, { 1.0f, 1.0f, 1.0f }, Quaternion::CreateFromEulerAngles(spindle ? 270_degrees : 0.0_radians, 0.0_radians, worldAngle), {});

	vertices->Unlock();
	indices->Unlock();

	return model;
}

auto Rendering::CreateDebugModel(Rectangle rectangle, bool spindle, Angle worldAngle, float z, Color color) -> Model*
{
	// This will eventually be gone and just standard debug drawing.

	auto vertices = _graphics.CreateGeometry(GraphicsStorage::TransferredToGpu);
	auto indices = _graphics.CreateGeometry(GraphicsStorage::TransferredToGpu); 
	vertices->Reset(GeometryTopology::TriangleList, 4);
	indices->Reset(GeometryTopology::IndexList, 6);

	auto builder = GeometryBuilder(*_coloredModelMaterial, *vertices, *indices);
	builder.DrawRectangle(rectangle, FillStyle(color), LineStyle(Black, 0.1f));

	auto model = Game()->Scene.AddModel();
	model->Tag = AllObjects;
	model->Surface.Material = _coloredModelMaterial->Id();
	model->Mesh.SetVertexBuffer(vertices->Retreive<ColoredModelVertex>(builder.VertexStart(), builder.VertexCount()));
	model->Mesh.SetIndexBuffer(indices->Retreive<uint16_t>(builder.IndexStart(), builder.IndexCount()));
	model->Transform = Matrix4x4::CreateTransform({ 0.0f, 0.0f, z }, { 1.0f, 1.0f, 1.0f }, Quaternion::CreateFromEulerAngles(spindle ? 270_degrees : 0.0_radians, 0.0_radians, worldAngle), {});

	vertices->Unlock();
	indices->Unlock();

	return model;
}
