#pragma once

#include <Pargon.h>

using namespace Pargon;

class GuiManager
{
public:
	GuiManager(GraphicsDevice& graphics);

	void Setup();
	void Render();

private:
	GraphicsDevice& _graphics;
};
