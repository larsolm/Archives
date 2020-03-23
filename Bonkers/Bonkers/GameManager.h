#pragma once

#include "Pargon/GameCore/GameObject.h"
#include "Pargon/Types/Random.h"
#include "Pargon/Types/List.h"
#include "Pargon/Text/TextObject.h"
#include "Pargon/Shapes/ShapeInstance.h"
#include "Pargon/Animation/PropertyAnimator.h"
#include "Pargon/GameCore/Scheduler.h"

namespace Bonkers
{
	struct PlayerProfile;
	class Player;
	class Board;
	class Deck;
	class Tile;
    
	class GameManager : public Pargon::GameObject
	{
		friend void Pargon::CreateMembers<GameManager>(Pargon::Reflection::Type* type);
	
	public:
		virtual void Start() override;

		void NewGame(const Pargon::List<std::unique_ptr<PlayerProfile>>& profiles, size_t points, size_t hand);
		void Roll(bool again);
		void AdvancePlayer(Player* player, int spacesLeft);

	private:
		size_t _pointsToWin = 10;
		size_t _numberOfPlayers = 4;
		size_t _handSize = 4;
		float _gameSpeed = 1.0f;
		float _playerSpeed = 0.25f;

		float _tileSize = 160;
		float _tileSpacing = 40;

		float _scoreSize = 200;
		float _scoreSpacing = 200;

		float _handX = 0;
		float _handY = -50;

		float _scoreY = 20;

		Pargon::List<Player*> _players;
		Pargon::List<Pargon::Shapes::ShapeInstance*> _scoreHighlights;

		Board* _board;
		Deck* _deck;
		Pargon::Text::TextObject* _directions;
		Pargon::Random _random;
		Pargon::Animation::PropertyAnimator* _animator;
		Pargon::Scheduler* _scheduler;

		size_t _turnIndex = 0;

		bool _diceRolled = false;
		bool _playedTile = false;

		bool _startTurn = true;
		bool _nextTurn = false;

		void NextTurn();
		void StartTurn();
		void EndTurn();
		void Continue();

		void RollDice();

		void TileClicked(size_t tileIndex);

		auto CheckCohabitation(Player* player) -> bool;
		void CheckSpace(Player* player);
		void EmptySpace(Player* player);
		void Lose(Player* player);
		void Score(Player* player, bool rollAgain);
		void FollowTile(Player* player, Tile* tile);

		auto GetScoreText(size_t index) -> Pargon::String;
	};
}

template<> void Pargon::CreateMembers<Bonkers::GameManager>(Pargon::Reflection::Type* type);
