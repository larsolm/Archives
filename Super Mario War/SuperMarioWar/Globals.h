#pragma once

#include "SuperMarioWar/SuperMarioWar.h"

namespace SuperMarioWar
{
	struct Globals_
	{
		Globals_();
		Pargon::Application::Core::Window Window;
		std::unique_ptr<Pargon::Input::KeyboardDevice> Keyboard;
		std::unique_ptr<Pargon::Input::ControllerDevice> Controller;
		std::unique_ptr<Pargon::Graphics::GraphicsDevice> Graphics;
		std::unique_ptr<Pargon::Gui::Context> Gui;
		std::unique_ptr<Pargon::Audio::AudioDevice> Audio;
		Pargon::Scripting::Interpreter* Scripting;
		std::unique_ptr<Pargon::Game::Core::Game> Game;
	};

	auto Globals() -> Globals_&;
}