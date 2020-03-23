#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/Tilemap/Tilemap.h"
#include "SuperMarioWar/Tilemap/TilemapInstance.h"
#include "SuperMarioWar/Tilemap/TilemapRenderer.h"

#include "SuperMarioWar/GameManager.h"
#include "SuperMarioWar/GameSettings.h"
#include "SuperMarioWar/Player.h"
#include "SuperMarioWar/Map.h"
#include "SuperMarioWar/CharacterSelectMenu.h"

using namespace Pargon;
using namespace SuperMarioWar;

void SuperMarioWar::Include()
{
	Types::Include();
	Math::Include();
	Reflection::Include();
	Serialization::Include();
	FileSystem::Include();
	Data::Include();
	Audio::Include();
	Graphics::Include();
	Input::Include();
	Scripting::Include();
	Networking::Include();
	Animation::Include();
	Text::Include();
	Gui::Include();
	Application::Core::Include();
	Game::Core::Include();
	Game::X2d::Include();
	Tools::Include();

	Reflection::AllTypes().Add<Tilemap>("SuperMarioWar::Tilemap");
	Game::Core::GameTypes().Add<TilemapObject, Pargon::Game::Core::GameObject>("SuperMarioWar::TilemapObject");
	Game::Core::GameTypes().Add<TilemapInstance, Pargon::Game::Core::Renderable>("SuperMarioWar::TilemapInstance");
	Game::Core::GameTypes().Add<TilemapRenderer, Pargon::Game::Core::Renderer>("SuperMarioWar::TilemapRenderer");

	Game::Core::GameTypes().Add<CharacterSelectMenu, Pargon::Gui::Menu>("SuperMarioWar::CharacterSelectMenu");
	Game::Core::GameTypes().Add<GameManager, Game::Core::Updater>("SuperMarioWar::GameManager");
	Game::Core::GameTypes().Add<PlayerType, void>("SuperMarioWar::PlayerType");
	Game::Core::GameTypes().Add<Player, Game::Core::Component>("SuperMarioWar::Player");
	Game::Core::GameTypes().Add<Map, Game::Core::GameObject>("SuperMarioWar::Map");
	Game::Core::GameTypes().Add<SpawnPoint, void>("SuperMarioWar::SpawnPoint");
}
