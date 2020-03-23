#pragma once

#include "Gui/GuiManager.h"
#include "Rendering/Rendering.h"
#include "World/World.h"
#include "Data/DataManager.h"
#include "Data/Settings.h"

#include <Pargon.h>

using namespace Pargon;

class Game_ : public GameInterface
{
public:
	Game_();
	~Game_();

	Log Log;
	Loader Loader;
	Application Application;
	GraphicsDevice Graphics;
	MouseDevice Mouse;
	KeyboardDevice Keyboard;
	ControllerDevice Controller;

	GuiManager Gui;
	DataManager Data;
	Rendering Rendering;
	World World;
	Scene Scene;

	Settings Settings;

	void Run();

	auto KeyboardState() const -> const KeyboardState&;
	auto MouseState() const -> const MouseState&;
	auto ControllerState() const -> const ControllerState&;

protected:
	void Setup() override;
	void Preprocess() override;
	void Start() override;
	void Activate() override;
	void Deactivate() override;
	void Stop() override;
	void Shutdown() override;

	void Update(Time elapsed) override;
	void Interpolate(float interpolation) override;
	void Animate(Time elapsed) override;
	void Render() override;

private:
	ApplicationInformation _application;
	RendererInformation _renderer;

	Pargon::KeyboardState _keyboard;
	Pargon::MouseState _mouse;
	Pargon::ControllerState _controller;
};

inline
auto Game_::KeyboardState() const -> const Pargon::KeyboardState&
{
	return _keyboard;
}

inline
auto Game_::MouseState() const -> const Pargon::MouseState&
{
	return _mouse;
}

inline
auto Game_::ControllerState() const -> const Pargon::ControllerState&
{
	return _controller;
}

auto Game() -> Game_*;
