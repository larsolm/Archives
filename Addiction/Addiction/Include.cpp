#include "Addiction/Addiction.h"

using namespace Pargon;

void Addiction::Include()
{
	Animation::Include();
	Debugging::Include();
	Application::Include();
	Reflection::Include();
	Serialization::Include();
	Types::Include();
	Math::Include();
	FileSystem::Include();
	Data::Include();
	Audio::Include();
	Graphics::Include();
	Input::Include();
	Scripting::Include();
	GameCore::Include();
	Game2d::Include();
	GameUi::Include();
	Shapes::Include();
	Text::Include();

	AddGameType<GameManager, GameObject>("Addiction::GameManager");
	AddGameType<Card, Component>("Addiction::Card");
}
