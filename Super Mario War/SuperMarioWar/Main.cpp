#include "SuperMarioWar/SuperMarioWar.h"

using namespace Pargon;
using namespace Pargon::Game;

auto main(int argc, char* argv[]) -> int
{
	SuperMarioWar::Include();

	GlobalLog().OnEntryAdded.Add(nullptr, std::function<void(const LogEntry&)>([](const LogEntry& entry)
	{
		if (entry.Messages.IsEmpty())
			return;

		std::cout << entry.Description.Characters() << "\n";
		for (auto& message : entry.Messages)
			std::cout << " - " << message.Characters() << "\n";
	}));

	auto& globals = SuperMarioWar::Globals();

	globals.Scripting = Scripting::GetLanguage("Squirrel");
	globals.Graphics = Graphics::CreateDefaultDevice(globals.Window, true);
	globals.Audio = Audio::CreateDefaultDevice(true);
	globals.Keyboard = Input::CreateDefaultKeyboardDevice(globals.Window);
	globals.Controller = Input::CreateDefaultControllerDevice(globals.Window);
	globals.Game = std::make_unique<Core::Game>(globals.Graphics.get(), globals.Scripting);
	globals.Gui = std::make_unique<Gui::Context>(globals.Graphics.get(), globals.Scripting, globals.Keyboard.get(), globals.Controller.get(), nullptr);

	auto networking = Networking::CreateDevice();
	Tools::Server server(networking.get());
	server.AddContext("Scripting", globals.Scripting);
	server.AddContext("Game", globals.Game.get());
	server.AddContext("Gui", globals.Gui.get());
	server.Startup(123456);

	globals.Game->LoadGame("Assets/SuperMarioWar.game");

	while (globals.Window.Run())
	{
		networking->Process();
		globals.Controller->Update();
		globals.Keyboard->Update();
		globals.Game->Update();
		globals.Gui->Update();
	}

	globals.Game.reset();

	return 0;
}
