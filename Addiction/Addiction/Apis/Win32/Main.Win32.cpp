#include "Addiction/Addiction.h"

#include "Pargon/Application/Apis/Win32/Win32.h"
#include "Pargon/Audio/Apis/XAudio2/XAudio2.h"
#include "Pargon/Graphics/Apis/Direct3d11/Direct3d11.h"
#include "Pargon/Graphics/Apis/Win32/Win32.h"
#include "Pargon/Input/Apis/Emulated/MouseToTouchEmulator.h"
#include "Pargon/Input/Apis/Win32/Win32KeyboardMouseDevice.h"
#include "Pargon/Input/Apis/Win32/XInputControllerDevice.h"

using namespace Pargon;
using namespace Pargon::Application;
using namespace Pargon::Audio;
using namespace Pargon::Graphics;
using namespace Pargon::Input;
using namespace Pargon::Networking;

int main(int argc, char* argv[])
{
	Addiction::Include();

	Win32Window window(1136, 600);
	Game game;

	game.AddGraphics<Direct3d11GraphicsDevice, Win32Direct3d11Canvas>(GetModuleHandle(0), window.Handle());
	game.AddAudio<XAudio2Device>();
	game.AddTouchInput<MouseToTouchEmulator<Win32KeyboardMouseDevice>>(window.Handle());
	game.AddControllerInput<XInputControllerDevice>();
	game.LoadGame("Addiction.game");

	window.Run();

	return 0;
}

int WINAPI WinMain(HINSTANCE instance, HINSTANCE previousInstance, LPSTR parameters, int show)
{
	return main(0, nullptr);
}
