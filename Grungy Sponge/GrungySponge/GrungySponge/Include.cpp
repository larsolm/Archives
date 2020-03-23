#include "GrungySponge/GrungySponge.h"

using namespace Pargon;

void GrungySponge::Include()
{
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
    
    //GrungySponge::Game::Include();
    //GrungySponge::Sponge::Include();
    //GrungySponge::Ui::Include();
    //GrungySponge::World::Include();
}
