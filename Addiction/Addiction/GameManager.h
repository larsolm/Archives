#pragma once

#include "Pargon/GameCore/GameObject.h"
#include "Pargon/GameCore/Scheduler.h"
#include "Pargon/Shapes/ShapeInstance.h"
#include "Pargon/Animation/PropertyAnimator.h"
#include "Pargon/Text/TextObject.h"
#include "Pargon/Types/Grid.h"
#include "Pargon/Types/List.h"
#include "Pargon/Types/Array.h"
#include "Pargon/Types/Random.h"
#include "Addiction/Card.h"

namespace Addiction
{
	enum class Suit
	{
		Spades,
		Hearts,
		Clubs,
		Diamonds
	};

	class GameManager : public Pargon::GameObject
	{
    friend void Pargon::CreateMembers<Addiction::GameManager>(Pargon::Reflection::Type* type);
        
	private:
		struct Move
		{
			size_t Row;
			size_t Column;
			Card* Card;
		};

		Pargon::Random _random;
		Pargon::Scheduler* _scheduler;
		Pargon::Text::TextObject* _winner;
		Pargon::Animation::PropertyAnimator* _animator;

		unsigned int _numberOfMoves;
		int _moveCounter;

		bool _isShuffling = false;

		Pargon::Array<Card*, 52> _cards;
		Pargon::Grid<Card*> _table;
		Pargon::Grid<Pargon::Shapes::ShapeInstance*> _highlights;
		Pargon::List<Card*> _deck;
		Pargon::List<Move> _undoStack;
        
        int _score = 0;
        
		float _moveTime = 0.3f;

		float _deckX = 0;
		float _deckY = 0;

        float _cardWidth = 65;
        float _cardHeight = 90;
		float _cardSpacing = 15;
		float _cardStartX = -480;
		float _cardStartY = 92;

		Card* GetCard(unsigned int value, Suit suit);
		auto GetCardPositionX(size_t row, size_t column) -> float;
		auto GetCardPositionY(size_t row, size_t column) -> float;
		void MoveCard(Card* card, size_t row, size_t column, float speed);

		auto UpdateLocked(size_t row) -> bool;
		auto CheckLocked(size_t row, size_t column) -> bool;
		auto CheckWin() -> bool;
		void Winner();

		void UpdateHighlights();
		void ClearHighlight(size_t row, size_t column);
		void ClearHighlights();
		void HighlightCard(size_t row, size_t column);

	public:
		~GameManager();

		void ToggleSound();
		void CardClicked(Card* selection);
		void Undo();

		void ClearTable();
		void ShufflePressed();
		void Shuffle();

		void NewGamePressed();
		void NewGame();

	protected:
		virtual void Start() override;
	};
}

template<> void Pargon::CreateMembers<Addiction::GameManager>(Pargon::Reflection::Type* type);
