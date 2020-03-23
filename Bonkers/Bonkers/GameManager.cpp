#include "Bonkers/Bonkers.h"

using namespace Bonkers;
using namespace Pargon;
using namespace Pargon::Shapes;
using namespace Pargon::Animation;
using namespace Pargon::Text;

template<> void Pargon::CreateMembers<Bonkers::GameManager>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<GameManager>();

	type->AddField("PointsToWin", &GameManager::_pointsToWin);
	type->AddField("GameSpeed", &GameManager::_gameSpeed);
	type->AddField("PlayerSpeed", &GameManager::_playerSpeed);
	type->AddField("NumberOfPlayers", &GameManager::_numberOfPlayers);
	type->AddField("HandSize", &GameManager::_handSize);
	type->AddMethod("RollDice", &GameManager::RollDice);
	type->AddMethod("TileClicked", &GameManager::TileClicked);
	type->AddMethod("Continue", &GameManager::Continue);
	type->AddMethod("GetScoreText", &GameManager::GetScoreText);
}

void GameManager::Start()
{
}

namespace
{
	auto CreatePlayer(PlayerProfile* profile)
	{
		auto playerEntity = dynamic_cast<Entity*>(CurrentGame()->CreateObject("PlayerTemplate"));
		auto player = playerEntity->GetComponent<Player>();
		player->CurrentSpace = 0;
		player->HasLoseCard = true;
		player->Score = 0;
		player->Name = profile->Name;
		player->TeamColor = profile->TeamColor;

		auto shape = playerEntity->GetComponent<ShapeInstance>();
		shape->Color = player->TeamColor;

		return player;
	}

	auto CreateHandTile(size_t index, float xPos, float yPos)
	{
		auto handTile = dynamic_cast<Ensemble*>(CurrentGame()->CreateObject("HandTileTemplate", "HandTile" + ToString(index)));

		auto indexComponent = handTile->GetComponent<IndexComponent>();
		indexComponent->Index = index;

		auto spatial = handTile->GetComponent<Spatial2D>();
		spatial->Position.X = xPos;
		spatial->Position.Y = yPos;
	}

	auto CreateScoreUi(Color color, float xPos, size_t index)
	{
		auto scoreEntity = dynamic_cast<Ensemble*>(CurrentGame()->CreateObject("ScoreTemplate"));

		auto anchor = scoreEntity->GetComponent<Anchor>();
		anchor->Offset.X = xPos;

		auto text = scoreEntity->FindComponent<TextObject>();
		text->FillColor = color;

		auto ind = scoreEntity->FindComponent<IndexComponent>();
		ind->Index = index;

		auto shape = scoreEntity->GetComponent<ShapeInstance>();
		
		return shape;
	}
}

namespace
{
	auto GetRowCount(size_t total, size_t index) -> size_t
	{
		switch (total)
		{
		case 6:
		case 8:
		case 10:
			return total / 2;
		case 7:
			if (index > 3)
				return 3;
			else
				return 4;
			return 4;
		case 9:
			if (index > 4)
				return 4;
			else
				return 5;
			return 5;
		default:
			return total;
		}
	}
}

void GameManager::NewGame(const List<std::unique_ptr<PlayerProfile>>& profiles, size_t points, size_t hand)
{
	_board = CurrentGame()->World().GetObjects<Board>().AllObjects().Last();
	_deck = CurrentGame()->World().GetObjects<Deck>().AllObjects().Last();
	_animator = CurrentGame()->World().GetObjects<PropertyAnimator>().AllObjects().Last();
	_scheduler = CurrentGame()->World().GetObjects<Scheduler>().AllObjects().Last();
	_directions = CurrentGame()->World().GetObjects<Entity>().ObjectNamed("Directions")->GetComponent<TextObject>();

	_board->InitializeBoard();
	_deck->InitializeDeck();

	_numberOfPlayers = profiles.Count();
	_pointsToWin = points;
	_handSize = hand;

	_players.Clear();

	for (auto i = size_t{ 0 }; i < _numberOfPlayers; i++)
	{
		auto player = _players.Add(CreatePlayer(profiles[i].get()));

		for (auto i = size_t{ 0 }; i < _handSize; i++)
			player->Hand.AddTile(_deck->DrawTile());

		auto spatial = player->Entity()->GetComponent<Spatial2D>();
		spatial->Position = _board->GetSpacePosition(0);

		auto xPos = ((i * spatial->Size.X) + (i * _scoreSpacing)) - (((_numberOfPlayers * spatial->Size.X) + ((_numberOfPlayers - 1) * _scoreSpacing)) / 2);

		_scoreHighlights.Add(CreateScoreUi(player->TeamColor, xPos, i));
	}

	for (auto i = size_t{ 0 }; i < _handSize; i++) 
	{
		auto xPos = _handX;
		auto yPos = _handY;
		auto count = GetRowCount(_handSize, i);
		auto width = (count * _tileSize) + (_tileSpacing * (count - 1));
		auto index = i;
		if (i >= count)
			index = i % count;
		xPos = (index * _tileSize) + (index * _tileSpacing);
		xPos -= width / 2;

		bool twoRows = _handSize > 5;
		if (twoRows)
		{
			auto first = i < count ? 1 : -1;
			yPos += ((_tileSize / 2) + (_tileSpacing / 2)) * first;
		}

		CreateHandTile(i, xPos, yPos);
	}

	_turnIndex = _numberOfPlayers - 1;
	NextTurn();
}

namespace
{
	void SetConfirmVisible(bool state)
	{
		auto entity = CurrentGame()->World().GetObjects<Entity>().ObjectNamed("Directions");
		auto directions = entity->GetComponent<TextObject>();

		auto ensemble = CurrentGame()->World().GetObjects<Ensemble>().ObjectNamed("ConfirmButton");
		auto shape = ensemble->GetComponent<ShapeInstance>();
		auto text = ensemble->FindComponent<TextObject>();

		directions->Visible = state;
		shape->Visible = state;
		text->Visible = state;
	}

	void SetDiceVisible(bool state)
	{
		auto ensemble = CurrentGame()->World().GetObjects<Ensemble>().ObjectNamed("DiceUi");
		auto background = ensemble->GetComponent<ShapeInstance>();
		auto text = ensemble->Children()[0]->GetComponent<TextObject>();
		auto diceOne = ensemble->Children()[1]->GetComponent<ShapeInstance>();
		auto diceTwo = ensemble->Children()[2]->GetComponent<ShapeInstance>();

		background->Visible = state;
		text->Visible = state;
		diceOne->Visible = state;
		diceTwo->Visible = state;
	}

	void UpdateTile(Ensemble* entity, Tile* tile)
	{
		auto text = entity->FindComponent<Text::TextObject>();
		text->Characters = tile->GetText();

		auto shape = entity->FindComponent<Shapes::ShapeInstance>();
		shape->Texture = tile->Icon();
	}

	void SetTileVisible(Ensemble* tile, bool state)
	{
		auto background = tile->GetComponent<ShapeInstance>();
		auto shape = tile->FindComponent<ShapeInstance>();
		auto text = tile->FindComponent<TextObject>();

		background->Visible = state;
		shape->Visible = state;
		text->Visible = state;
	}

	void SetHandVisible(Player* player, size_t count, bool state)
	{
		auto ensemble = CurrentGame()->World().GetObjects<Ensemble>().ObjectNamed("HandBackground");
		auto background = ensemble->GetComponent<ShapeInstance>();
		auto text = ensemble->FindComponent<TextObject>();

		background->Visible = state;
		text->Visible = state;

		for (auto i = size_t{ 0 }; i < count; i++)
		{
			auto tileEntity = CurrentGame()->World().GetObjects<Ensemble>().ObjectNamed("HandTile" + ToString(i));
			SetTileVisible(tileEntity, state);

			if (state)
			{
				auto tile = player->Hand.GetTile(i);
				UpdateTile(tileEntity, tile);
			}
		}
	}
}

void GameManager::NextTurn()
{
	_scoreHighlights[_turnIndex]->Visible = false;
	_turnIndex++;

	if (_turnIndex >= _numberOfPlayers)
		_turnIndex = 0;

	_scoreHighlights[_turnIndex]->Visible = true;

	_directions->Characters = "Player " + ToString(_turnIndex + 1) + "'s Turn.";

	_startTurn = true;
	SetConfirmVisible(true);
}

void GameManager::StartTurn()
{
	while (_players[_turnIndex]->Hand.TileCount() < _handSize)
		_players[_turnIndex]->Hand.AddTile(_deck->DrawTile());

	_playedTile = false;

	Roll(false);
}

void GameManager::EndTurn()
{
	_directions->Characters = "Your turn is over.";

	_nextTurn = true;
	SetConfirmVisible(true);
}

void GameManager::Roll(bool again)
{
	auto text = CurrentGame()->World().GetObjects<Ensemble>().ObjectNamed("DiceUi")->Children()[0]->GetComponent<TextObject>();
	if (again)
		text->Characters = "Roll again!";
	else
		text->Characters = "Roll the dice!";

	SetDiceVisible(true);
	_diceRolled = false;
}

void GameManager::RollDice()
{
	if (_diceRolled)
		return;

	_board->ClearHit();

	_diceRolled = true;

	auto diceOne = _random.Generate(1, 6);
	auto diceTwo = _random.Generate(1, 6);

	auto total = diceOne + diceTwo;

	auto diceUi = CurrentGame()->World().GetObjects<Ensemble>().ObjectNamed("DiceUi");

	auto shapeOne = diceUi->Children()[1]->GetComponent<ShapeInstance>();
	auto shapeTwo = diceUi->Children()[2]->GetComponent<ShapeInstance>();

	shapeOne->Texture = "Dice" + ToString(diceOne) + "Texture";
	shapeTwo->Texture = "Dice" + ToString(diceTwo) + "Texture";

	_scheduler->Schedule(_gameSpeed, &GameManager::AdvancePlayer, this, _players[_turnIndex], total);
	_scheduler->Schedule(_gameSpeed, SetDiceVisible, false);
}

void GameManager::AdvancePlayer(Player* player, int spacesLeft)
{
	auto sign = spacesLeft > 0 ? 1 : -1;

	auto currentSpace = static_cast<int>(player->CurrentSpace);
	currentSpace += sign;
	spacesLeft -= sign;

	if (currentSpace > static_cast<int>(_board->TotalSpaces()) - 1)
		currentSpace = 0;
	else if (currentSpace < 0)
		currentSpace = _board->TotalSpaces() - 1;

	player->CurrentSpace = static_cast<size_t>(currentSpace);

	auto spatial = player->Entity()->GetComponent<Spatial2D>();
	auto spacePosition = _board->GetSpacePosition(player->CurrentSpace);

	_animator->Transition(spatial, spatial->Position, spacePosition, _playerSpeed);

	if (spacesLeft != 0)
		_scheduler->Schedule(_playerSpeed, &GameManager::AdvancePlayer, this, player, spacesLeft);
	else
		_scheduler->Schedule(_playerSpeed, &GameManager::CheckSpace, this, player);
}

void GameManager::CheckSpace(Player* player)
{
	auto& space = _board->GetSpace(player->CurrentSpace);

	if (space.Hit())
	{
		Score(player, CheckCohabitation(player));
		return;
	}

	space.SetHit(true);

	switch (space.Type())
	{
	case SpaceType::Empty:
		if (CheckCohabitation(player))
			Roll(true);
		else
			EmptySpace(player);
		break;
	case SpaceType::Lose:
		Lose(player);
		break;
	case SpaceType::Score:
		Score(player, CheckCohabitation(player));
		break;
	case SpaceType::Tiled:
		FollowTile(player, space.CurrentTile());
		break; 
	}
}

void GameManager::FollowTile(Player* player, Tile* tile)
{
	tile->Process(this, player, _board);
}

void GameManager::EmptySpace(Player* player)
{
	if (_playedTile)
	{
		if (CheckCohabitation(player))
			Roll(true);
		else
			EndTurn();
	}
	else
	{
		SetHandVisible(player, _handSize, true);
	}
}

void GameManager::TileClicked(size_t tileIndex)
{
	if (_playedTile)
		return;

	auto player = _players[_turnIndex];
	auto tile = player->Hand.GetTile(tileIndex);

	_board->PlaceTile(tile, player->CurrentSpace);

	player->Hand.RemoveTile(tileIndex);

	// Transition tile to board.
	_playedTile = true;
	SetHandVisible(player, _handSize, false);
	_scheduler->Schedule(_gameSpeed, &GameManager::FollowTile, this, player, tile);
}

void GameManager::Lose(Player* player)
{
	player->Score--;
	_directions->Characters = "You Lose..";

	_scheduler->Schedule(_gameSpeed, &GameManager::EndTurn, this);
}

void GameManager::Score(Player* player, bool rollAgain)
{
	player->Score++;
	if (player->Score >= _pointsToWin)
	{
		// Win gui stuff here
	}

	if (rollAgain)
		Roll(true);
	else
		EndTurn();
}

auto GameManager::CheckCohabitation(Player* player) -> bool
{
	for (auto otherPlayer : _players)
	{
		if (player != otherPlayer)
		{
			if (player->CurrentSpace == otherPlayer->CurrentSpace)
				return true;
		}
	}

	return false;
}

void GameManager::Continue()
{
	if (_startTurn)
	{
		SetConfirmVisible(false);
		_startTurn = false;
		StartTurn();
	}
	else if (_nextTurn)
	{
		SetConfirmVisible(false);
		_nextTurn = false;
		NextTurn();
	}
}

auto GameManager::GetScoreText(size_t index) -> Pargon::String
{
	auto text = Pargon::String();
	auto player = _players[index];

	text = text + player->Name + ": " + ToString(player->Score);
	return text;
}
