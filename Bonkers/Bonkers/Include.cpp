#include "Bonkers/Bonkers.h"

using namespace Pargon;

void Bonkers::Include()
{
	Animation::Include();
	Debugging::Include();
	ApplicationCore::Include();
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

	Reflection::AllTypes().Add<CubicBezierPath>("Bonkers::CubicBezierPath");
	Reflection::AllTypes().Add<CubicBezierPathNode>("Bonkers::CubicBezierPathNode");

	Reflection::AllTypes().Add<Tile>("Bonkers::Tile");
	Reflection::AllTypes().Add<GoToScoreTile, Tile>("Bonkers::GoToScoreTile");
	Reflection::AllTypes().Add<GoToStartTile, Tile>("Bonkers::GoToStartTile");
	Reflection::AllTypes().Add<MoveTile, Tile>("Bonkers::MoveTile");
	Reflection::AllTypes().Add<RollAgainTile, Tile>("Bonkers::RollAgainTile");

	AddGameType<GameManager, GameObject>("Bonkers::GameManager");
	AddGameType<MainMenu, KeyboardMouseMenu>("Bonkers::MainMenu");
	AddGameType<PlayerSelect, KeyboardMouseMenu>("Bonkers::PlayerSelect");
	AddGameType<Player, Component>("Bonkers::Player");
	AddGameType<Board, GameObject>("Bonkers::Board");
	AddGameType<Deck, GameObject>("Bonkers::Deck");
	AddGameType<BoardRenderer, Renderer>("Bonkers::BoardRenderer");
	AddGameType<BoardRenderable, Renderable>("Bonkers::BoardRenderable");
}
