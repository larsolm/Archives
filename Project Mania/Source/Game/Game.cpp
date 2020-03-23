#include "Pch.h"
#include "Game/Game.h"

using namespace Pargon;

namespace
{
	static Game_* _game = nullptr;
}

auto Game() -> Game_*
{
	return _game;
}

Game_::Game_() :
	Application(),
	Graphics(),
	Gui(Graphics),
	Rendering(Graphics),
	Scene(Graphics)
{
	assert(_game == nullptr);
	_game = this;
}

Game_::~Game_()
{
	assert(_game == this);
	_game = nullptr;
}

void Game_::Run()
{
	_application = Application.Initialize("Project Mania", *this);
	_application.WriteInformation(Log);
	_application.WriteCapabilities(Log);

	Application.Run({ _application.Name, WindowStyle::Static, (_application.ScreenWidth - 1920) >> 1, (_application.ScreenHeight - 1080) >> 1, 1920, 1080 });
}

void Game_::Setup()
{
	Loader.AddTask(std::make_unique<PreprocessTask>(*this), 0);

	_renderer = Graphics.Setup(Application, _application.AvailableRenderers.First());
	_renderer.WriteInformation(Log);
	_renderer.WriteCapabilities(Log);

	Mouse.Setup(Application);
	Keyboard.Setup(Application);
	Controller.Setup({});

	Data.Load();
	Rendering.Setup();
	World.Load();
}

void Game_::Preprocess()
{
}

void Game_::Start()
{
}

void Game_::Activate()
{
}

void Game_::Deactivate()
{
}

void Game_::Stop()
{
}

void Game_::Shutdown()
{
}

void Game_::Update(Time elapsed)
{
	Keyboard.Update(_keyboard);
	Mouse.Update(_mouse);
	Controller.Update(_controller);

	World.Update(elapsed);
}

void Game_::Interpolate(float interpolation)
{
	Rendering.Interpolate(interpolation);
}

void Game_::Animate(Time elapsed)
{
	Rendering.Animate(elapsed);
}

void Game_::Render()
{
	Rendering.Render();
	Gui.Render();

	Graphics.Render(GraphicsDevice::NoSynchronization);
}
