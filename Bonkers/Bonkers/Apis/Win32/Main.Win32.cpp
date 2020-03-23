#include "Bonkers/Bonkers.h"

#include "Pargon/ApplicationCore/Apis/Win32/Win32.h"
#include "Pargon/Audio/Apis/DirectSound/DirectSound.h"
#include "Pargon/Graphics/Apis/Direct3d11/Direct3d11.h"
#include "Pargon/Graphics/Apis/Win32/Win32.h"
#include "Pargon/Input/Apis/Win32/Win32KeyboardMouseDevice.h"
#include "Pargon/Input/Apis/Win32/XInputControllerDevice.h"

using namespace Pargon;
using namespace Pargon::ApplicationCore;
using namespace Pargon::Audio;
using namespace Pargon::Graphics;
using namespace Pargon::Input;
using namespace Pargon::Networking;

int main(int argc, char* argv[])
{
	Bonkers::Include();

	Win32Window window(1280, 720);
	Game game;

	game.AddGraphics<Direct3d11GraphicsDevice, Win32Direct3d11Canvas>(GetModuleHandle(0), window.Handle());
	game.AddAudio<DirectSoundDevice>(window.Handle()); 
	game.AddKeyboardMouseInput<Win32KeyboardMouseDevice>(window.Handle());
	game.AddControllerInput<XInputControllerDevice>();
	game.LoadGame("Bonkers.game");

	window.Run();

	return 0;
}

int WINAPI WinMain(HINSTANCE instance, HINSTANCE previousInstance, LPSTR parameters, int show)
{
	return main(0, nullptr);
}
