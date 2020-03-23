#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace Pargon::Text;
using namespace GrungySponge;

void GameScreen::Start()
{
	TouchMenu::Start();
	_gameManager->LoadLevel();

	Entity* display = CurrentGame()->World().GetObjects<Entity>().ObjectNamed("MinutesDisplay");
	_minutesText = display->GetComponent<TextObject>();
	display = CurrentGame()->World().GetObjects<Entity>().ObjectNamed("SecondsDisplay");
	_secondsText = display->GetComponent<TextObject>();
	display = CurrentGame()->World().GetObjects<Entity>().ObjectNamed("TimerDividerDisplay");
	_dividerText = display->GetComponent<TextObject>();

	int index = 0;
	float startY = 0.0f;
	float spacing = 0.0f;

	if (_gameManager->CurrentLevel.Laps > 0)
	{
        display = static_cast<Entity*>(CurrentGame()->CreateObject("CounterDisplayTemplate", "LapsDisplay"));
		_lapsText = display->GetComponent<TextObject>();

		Spatial2D* spatial = display->GetComponent<Spatial2D>();
        Anchor* anchor = display->GetComponent<Anchor>();
		
		startY = anchor->Offset.Y;
		spacing = spatial->Size.Y / 3;
		
		index++;
	}

	if (_gameManager->CurrentLevel.ObjectsToFind > 0)
	{
		display = static_cast<Entity*>(CurrentGame()->CreateObject("CounterDisplayTemplate", "CollectablesDisplay"));
		_collectablesText = display->GetComponent<TextObject>();

		Spatial2D* spatial = display->GetComponent<Spatial2D>();
		Anchor* anchor = display->GetComponent<Anchor>();

		if (index > 0)
		{
			anchor->Offset.Y = startY - (index * (spatial->Size.Y + spacing));
		}
		else
		{
			startY = anchor->Offset.Y;
			spacing = spatial->Size.Y / 3;
		}
		index++;
	}

	if (_gameManager->CurrentLevel.UncoverableObjects > 0)
	{
		display = static_cast<Entity*>(CurrentGame()->CreateObject("CounterDisplayTemplate", "UncoverablesDisplay"));
		_uncoverablesText = display->GetComponent<TextObject>();

		Spatial2D* spatial = display->GetComponent<Spatial2D>();
		Anchor* anchor = display->GetComponent<Anchor>();

		if (index > 0)
		{
			anchor->Offset.Y = startY - (index * (spatial->Size.Y + spacing));
		}
		else
		{
			startY = anchor->Offset.Y;
			spacing = spatial->Size.Y / 3;
		}
		index++;
	}

	if (_gameManager->CurrentLevel.PercentageToClear > 0)
	{
		display = static_cast<Entity*>(CurrentGame()->CreateObject("CounterDisplayTemplate", "PercentageDisplay"));
		_percentageText = display->GetComponent<TextObject>();

		Spatial2D* spatial = display->GetComponent<Spatial2D>();
		Anchor* anchor = display->GetComponent<Anchor>();

		if (index > 0)
		{
			anchor->Offset.Y = startY - (index * (spatial->Size.Y + spacing));
		}
		else
		{
			startY = anchor->Offset.Y;
			spacing = spatial->Size.Y / 3;
		}
	}

	if (_gameManager->CurrentLevel.DirtCapacity > 0)
	{
		Entity* capacity = static_cast<Entity*>(CurrentGame()->CreateObject("CapacityOutline"));
		Spatial2D* outlineSpatial = capacity->GetComponent<Spatial2D>();
		_capacitySize = outlineSpatial->Size.X;
		capacity = static_cast<Entity*>(CurrentGame()->CreateObject("CapacityDisplay"));
		_capacitySpatial = capacity->GetComponent<Spatial2D>();
	}
}

void GameScreen::Activate()
{
	TouchMenu::Activate();

	_timer->Start();
}

void GameScreen::Deactivate()
{
	TouchMenu::Deactivate();

	_timer->Stop();
}

void GameScreen::Update(float elapsed)
{
	TouchMenu::Update(elapsed);

	if (_gameManager->Paused)
		return;

	_gameManager->Update();

	TimerGUI();
	CounterGUI();
	CapacityGUI();
}

void GameScreen::TimerGUI()
{
	_minutesText->Characters = _timer->MinutesText();
	_secondsText->Characters = _timer->SecondsText();

	_gameManager->IsGold = _timer->TotalTime() <= _gameManager->CurrentLevel.GoldTime;
	_gameManager->IsSilver = _timer->TotalTime() <= _gameManager->CurrentLevel.SilverTime;
	_gameManager->IsBronze = _timer->TotalTime() <= _gameManager->CurrentLevel.BronzeTime;

	if (_gameManager->IsGold)
	{
		_minutesText->StrokeColor = Color(255, 215, 0);
		_secondsText->StrokeColor = Color(255, 215, 0);
		_dividerText->StrokeColor = Color(255, 215, 0);
	}
	else if (_gameManager->IsSilver)
	{
		_minutesText->StrokeColor = Color(204, 204, 204);
		_secondsText->StrokeColor = Color(204, 204, 204);
		_dividerText->StrokeColor = Color(204, 204, 204);
	}
	else if (_gameManager->IsBronze)
	{
		_minutesText->StrokeColor = Color(150, 90, 56);
		_secondsText->StrokeColor = Color(150, 90, 56);
		_dividerText->StrokeColor = Color(150, 90, 56);
	}
	else
	{
		_minutesText->StrokeColor = Color(255, 0, 0);
		_secondsText->StrokeColor = Color(255, 0, 0);
		_dividerText->StrokeColor = Color(255, 0, 0);
	}
}

void GameScreen::CounterGUI()
{
	if (_lapsText != nullptr)
	{
		int laps = _gameManager->LapsCompleted >= 0 ? _gameManager->LapsCompleted : 0;
		_lapsText->Characters = "Lap: " + ToString(laps + 1) + "/" + ToString(_gameManager->CurrentLevel.Laps);
	}

	if (_collectablesText != nullptr)
	{
		_collectablesText->Characters = "Found: " + ToString(_gameManager->FoundObjects) + "/" + ToString(_gameManager->CurrentLevel.ObjectsToFind);
	}

	if (_uncoverablesText != nullptr)
	{
		_uncoverablesText->Characters = "Uncovered: " + ToString(_gameManager->UncoveredObjects) + "/" + ToString(_gameManager->CurrentLevel.UncoverableObjects);
	}

	if (_gameManager->CurrentLevel.PercentageToClear > 0)
	{
		_percentageText->Characters = ToString(static_cast<int>(_gameManager->ClearedDirtPercent())) + "%";
	}
}

void GameScreen::CapacityGUI()
{
	if (_capacitySpatial != nullptr)
		_capacitySpatial->Size.X = _capacitySize * _gameManager->SpongeCapacity();
}
