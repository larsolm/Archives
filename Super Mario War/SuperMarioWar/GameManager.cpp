#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/GameManager.h"

using namespace SuperMarioWar;
using namespace Pargon;

void GameManager::Start()
{
	Globals().Scripting->AddGlobalVariable("GameSettings", &_gameSettings);
	auto guiCamera = _guiCameras->AllObjects().Last();
	guiCamera->SetContext(Globals().Gui.get());
	Globals().Gui->PushGui("Assets/Guis/mainMenu.gui");
}

void GameManager::Exit()
{
	Globals().Window.Close();
}

void GameManager::StartGame()
{
	Globals().Game->LoadLevel("Game");
	Globals().Gui->PushGui("Assets/Guis/gameScreen.gui");

	_mapName = "TestMap";

	Globals().Game->LoadFile("Assets/Maps/" + _mapName + ".level");
	_map = _maps->ObjectNamed(_mapName);

	_map->LoadMap();

	CreatePlayers();
	_playerManager.StartGame();

	for (auto player : _players)
		SpawnPlayer(player);
}

void GameManager::CreatePlayers()
{
	int index = -1;
	int controller = 0;

	for (int i = 0; i < 4; i++)
	{
		index++;
		auto& settings =  _gameSettings.GetPlayerSettings(i);

		if (settings.PlayerType == PlayerType::Inactive)
		{
			index--;
			continue;
		}

		auto entity = static_cast<Game::Core::Entity*>(Globals().Game->CreateObject("Player", "Player" + i));
		auto player = entity->GetComponent<Player>();
		player->PlayerIndex = index;

		if (settings.PlayerType == PlayerType::Human)
		{
			player->ControllerIndex = controller;
			player->InputType = settings.InputType;
			if (settings.InputType == InputType::Controller)
				controller++;
		}
		else if (settings.PlayerType == PlayerType::Bot)
		{
		}

		_players.Add(player);
	}
}

void GameManager::SpawnPlayer(Player* player)
{
	player->Velocity.Set(0, 0);
	player->State = PlayerState::Spawned;
	player->Invincibility = 1.0f;

	auto index = _random.GenerateInt(0, _map->SpawnPoints.Count() - 1);
	auto& spawnPoint = _map->SpawnPoints[index];
	auto position = _map->GetMapPosition(spawnPoint.Column, spawnPoint.Row);
	auto spatial = player->Entity()->GetComponent<Game::X2d::Spatial>();

	spatial->Position = position;
}

void GameManager::KillPlayer(Player* victim, Player* assailant)
{
	assailant->Kills++;
	assailant->Score++;
	victim->Deaths++;

	victim->State = PlayerState::Dead;
	victim->Respawn = 3.0f;
	victim->Velocity.Set(0.0f, 400.0f);

	auto sound = _sounds->ObjectNamed(victim->DeathSound);
	Globals().Audio->PlaySound(**sound, false, 1.0f);
}

void GameManager::Update(float elapsed)
{
	for (auto player : _players)
	{
		if (player->State == PlayerState::Dead)
		{
			player->Respawn -= elapsed;
			if (player->Respawn < 0.0f)
				SpawnPlayer(player);
		}
	}

	_playerManager.UpdatePlayers(elapsed);
	_collisionManager.ResolveTilemaps(elapsed);
	_collisionManager.ResolveStomps(elapsed, this);
}

template<>
void Pargon::CreateMembers<GameManager>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<GameManager>();

	type->AddMethod("StartGame", &GameManager::StartGame);
	type->AddMethod("Score", &GameManager::Score);
	type->AddMethod("Exit", &GameManager::Exit);
}
