#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace Pargon::Shapes;
using namespace GrungySponge;

void GameManager::LoadLevel()
{
	_levelFinished = false;
	IsGold = true;
	IsSilver = true;
	IsBronze = true;

	Pargon::String levelPath = Directory().Move("Worlds").MoveDown(CurrentWorld.Name).GetFile(CurrentLevel.Filename).PathAndFilename();
	CurrentGame()->LoadFile(levelPath);

	LapsCompleted = 0;
	FoundObjects = 0;
	UncoveredObjects = 0;

	DirtCapacity = 0;
	for (int i = 0; i < 3; i++)
		HeldDirt[i] = 0;

	_clearedDirtPercent = 0;
	_clearFieldManager->LoadLevel();
	_dirtManager->LoadLevel(CurrentLevel, _clearFieldManager);

    CurrentLevel.ObjectsToFind = CurrentGame()->World().GetObjects<CollisionComponent>().ObjectsWithTag("Collectable").Count();
	CurrentLevel.UncoverableObjects = CurrentGame()->World().GetObjects<ClearField>().ObjectsWithTag("Uncoverable").Count();
	
	SpongeData* sponge = CurrentGame()->World().GetObjects<SpongeData>().AllObjects().Last();
	Spatial2D* spatial = sponge->Entity()->GetComponent<Spatial2D>();
	spatial->Position = CurrentLevel.StartingPosition;
	spatial->Rotation = 0;
	sponge->CurrentType = CurrentLevel.SpongeType;
	sponge->MinimumSize = spatial->Size;

	SpongeCamera* camera = CurrentGame()->World().GetObjects<SpongeCamera>().AllObjects().Last();
	camera->WorldBoundsBottom = static_cast<int>(CurrentLevel.CameraBounds.Y);
	camera->WorldBoundsLeft = static_cast<int>(CurrentLevel.CameraBounds.X);
	camera->WorldBoundsRight = static_cast<int>(CurrentLevel.CameraBounds.X + CurrentLevel.CameraBounds.Width);
	camera->WorldBoundsTop = static_cast<int>(CurrentLevel.CameraBounds.Y + CurrentLevel.CameraBounds.Height);
	camera->Position = spatial->Position;
}

void GameManager::UnloadLevel()
{
	_levelFinished = false;

	CurrentLevel.Laps = 0;
	CurrentLevel.UncoverableObjects = 0;
	CurrentLevel.ObjectsToFind = 0;
	CurrentLevel.PercentageToClear = 0;

	Pargon::String levelPath = Directory().Move("Levels/Worlds").MoveDown(CurrentWorld.Name).GetFile(CurrentLevel.Filename).PathAndFilename();
	CurrentGame()->UnloadFile(levelPath);
}

void GrungySponge::GameManager::RestartLevel()
{
	UnloadLevel();
	LoadLevel();
}

void GameManager::NextLevel()
{
}

void GameManager::Update()
{
	if (Paused)
		return;

	_clearedDirtPercent = _dirtManager->CalculateClearedDirtPercent(_clearFieldManager);
	DirtCapacity = _dirtManager->CalculateDirtCapacity(HeldDirt);
	ResolveVictoryConditions();

	if (_levelFinished)
	{
		UiStack* uiStack = CurrentGame()->World().GetObjects<UiStack>().ObjectNamed("UiStack");
		uiStack->Push("PostGame", true);
	}
}

void GameManager::Calibrate()
{
	CurrentGame()->MotionInput()->CalibrateMotion();
}

void GameManager::ResolveVictoryConditions()
{
	if (CurrentLevel.Laps > 0)
	{
		if (LapsCompleted >= CurrentLevel.Laps)
		{
			_levelFinished = true;
			if (CurrentLevel.ObjectsToFind > 0)
			{
				if (FoundObjects <= CurrentLevel.ObjectsToFind)
				{
					IsBronze = false;
					IsSilver = false;
					IsGold = false;
				}
			}
			else if (CurrentLevel.UncoverableObjects > 0)
			{
				if (UncoveredObjects <= CurrentLevel.UncoverableObjects)
				{
					IsBronze = false;
					IsSilver = false;
					IsGold = false;
				}
			}
			else if (CurrentLevel.PercentageToClear > 0)
			{
				if (_clearedDirtPercent <= CurrentLevel.PercentageToClear)
				{
					IsBronze = false;
					IsSilver = false;
					IsGold = false;
				}
			}
		}
	}
	else if (CurrentLevel.ObjectsToFind > 0)
	{
		_levelFinished = FoundObjects >= CurrentLevel.ObjectsToFind;
	}
	else if (CurrentLevel.UncoverableObjects > 0)
	{
		_levelFinished = UncoveredObjects >= CurrentLevel.UncoverableObjects;
	}
	else if (CurrentLevel.PercentageToClear > 0)
	{
		_levelFinished = _clearedDirtPercent >= CurrentLevel.PercentageToClear;
	}
}

float GameManager::SpongeCapacity() const
{
	return static_cast<float>(DirtCapacity) / _dirtManager->LevelDirtCapacity();
}

template<> void Pargon::CreateMembers<GrungySponge::GameManager>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<GameManager>();

	type->AddField("Paused", &GameManager::Paused);
	type->AddField("CurrentWorld", &GameManager::CurrentWorld);
	type->AddField("CurrentLevel", &GameManager::CurrentLevel);
	type->AddField("IsGold", &GameManager::IsGold);
	type->AddField("IsSilver", &GameManager::IsSilver);
	type->AddField("IsBronze", &GameManager::IsBronze);
	type->AddMethod("LoadLevel", &GameManager::LoadLevel);
	type->AddMethod("UnloadLevel", &GameManager::UnloadLevel);
	type->AddMethod("RestartLevel", &GameManager::RestartLevel);
	type->AddMethod("Calibrate", &GameManager::Calibrate);
}
