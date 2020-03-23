#pragma once

#include "Rendering/PlayerCamera.h"

#include <Pargon.h>
#include <PargonScene.h>
#include <PargonShapes.h>

using namespace Pargon;

struct TexturedModelVertex
{
	Vector3 Position;
	Vector3 Normal;
	Vector3 Tangent;
	Vector3 Binormal;
	Vector2 Coordinate;
};

struct ColoredModelVertex
{
	Vector3 Position;
	Color Color;
};

class Rendering
{
public:
	static constexpr int AllObjects = 0x1;

	PlayerCamera Camera;

	Rendering(GraphicsDevice& graphics);

	void Setup();
	void Interpolate(float interpolation);
	void Animate(Time elapsed);
	void Render();

	auto CreateModel() -> Model*;
	auto CreateDebugModel(Shapes::Segment2 line, bool spindle, Angle worldAngle, float z, Color color) -> Model*;
	auto CreateDebugModel(Shapes::Rectangle recangle, bool spindle, Angle worldAngle, float z, Color color) -> Model*;

private:
	GraphicsDevice& _graphics;

	Material* _texturedModelMaterial;
	Material* _coloredModelMaterial;

	Texture* _diffuse;
	Texture* _normal;
	Texture* _occlusion;
	Texture* _roughness;
	Texture* _metallic;
};
