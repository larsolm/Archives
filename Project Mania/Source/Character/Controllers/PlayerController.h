#pragma once

#include "Character/Controller.h"

#include <Pargon.h>

using namespace Pargon;

class PlayerController : public Controller
{
public:
	void Update(float elapsed) override;

private:
	void Update(const ControllerState& controller, int controllerIndex);
	void Update(const KeyboardState& keyboard, const MouseState& mouse);
};
