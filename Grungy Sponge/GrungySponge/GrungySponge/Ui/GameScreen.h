#pragma once

#include "Pargon/GameUi/Touch/TouchMenu.h"
#include "Pargon/GameCore/GameCore.h"
#include "GrungySponge/Game/GameManager.h"
#include "GrungySponge/Ui/Timer.h"

namespace Pargon
{
	class Spatial2D;

	namespace Text
	{
		class TextObject;
	}
}

namespace GrungySponge
{
	class GameScreen : public Pargon::TouchMenu
	{
	private:
		Pargon::Text::TextObject* _minutesText;
		Pargon::Text::TextObject* _dividerText;
		Pargon::Text::TextObject* _secondsText;

		Pargon::Text::TextObject* _percentageText = nullptr;
		Pargon::Text::TextObject* _collectablesText = nullptr;
		Pargon::Text::TextObject* _uncoverablesText = nullptr;
		Pargon::Text::TextObject* _lapsText = nullptr;

		Pargon::Spatial2D* _capacitySpatial = nullptr;
		float _capacitySize;

		Pargon::GameObjectReference<GameManager> _gameManager;
		Pargon::GameObjectReference<Timer> _timer;

	public:
		virtual ~GameScreen() {};
        
		void TimerGUI();
		void CounterGUI();
		void CapacityGUI();
		
		virtual void Update(float elapsed) override;
		virtual void Activate();
		virtual void Deactivate();

	protected:
		virtual void Start() override;
	};
}

