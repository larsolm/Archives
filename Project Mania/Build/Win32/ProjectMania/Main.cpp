#include "Pch.h"
#include "Game/Game.h"

#define NOMINMAX
#define WIN32_LEAN_AND_MEAN

#include <Windows.h>

auto CALLBACK WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) -> int
{
	Game_ game;
	game.Run();

	return 0;
}
