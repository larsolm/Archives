#include "Addiction/Addiction.h"

using namespace Pargon;
using namespace Pargon::Shapes;
using namespace Pargon::Text;
using namespace Pargon::Animation;
using namespace Addiction;

template<> void Pargon::CreateMembers<Addiction::GameManager>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<GameManager>();

	type->AddField("Score", &GameManager::_score);
    type->AddField("CardWidth", &GameManager::_cardWidth);
    type->AddField("CardHeight", &GameManager::_cardHeight);
    type->AddField("CardSpacing", &GameManager::_cardSpacing);
    type->AddField("CardStartX", &GameManager::_cardStartX);
    type->AddField("CardStartY", &GameManager::_cardStartY);
    type->AddField("DeckX", &GameManager::_deckX);
    type->AddField("DecxY", &GameManager::_deckY);
    
	type->AddMethod("New", &GameManager::NewGamePressed);
	type->AddMethod("Shuffle", &GameManager::ShufflePressed);
	type->AddMethod("Undo", &GameManager::Undo);
	type->AddMethod("CardClicked", &GameManager::CardClicked);
	type->AddMethod("ToggleSound", &GameManager::ToggleSound);
}

void GameManager::Start()
{
	_table.SetSize(4, 13);
	_highlights.SetSize(4, 13);

	for (size_t i = 0; i < _cards.Count(); i++)
 
	{
 		Ensemble* cardEntity = static_cast<Ensemble*>(CurrentGame()->CreateObject("CardTemplate"));
		Card* card = cardEntity->GetComponent<Card>();
		_cards[i] = card;

		ShapeInstance* cardShape = cardEntity->GetComponent<ShapeInstance>();
		cardShape->Depth = 1.1f + (static_cast<float>(i) / 100);

		ShapeInstance* suitShape = cardEntity->Children()[0]->GetComponent<ShapeInstance>();

		if (i < 13)
		{
			card->Suit = Suit::Spades;
			suitShape->Texture = "SpadesTexture";
		}
		else if (i < 26)
		{
			card->Suit = Suit::Hearts;
			suitShape->Texture = "HeartsTexture";
		}
		else if (i < 39)
		{
			card->Suit = Suit::Clubs;
			suitShape->Texture = "ClubsTexture";
		}
		else
		{
			card->Suit = Suit::Diamonds;
			suitShape->Texture = "DiamondsTexture";
		}

		suitShape->Depth = 1 + (static_cast<float>(i) / 100);

		TextObject* character = cardEntity->Children()[1]->GetComponent<TextObject>();

		int mod = (i % 13) + 1;

		if (mod == 1)
			character->Characters = "A";
		else if (mod == 10)
			character->Characters = "10";
		else if (mod == 11)
			character->Characters = "J";
		else if (mod == 12)
			character->Characters = "Q";
		else if (mod == 13)
			character->Characters = "K";
		else
			character->Characters = ToString(mod);

		character->Depth = 1 + (static_cast<float>(i) / 100);

		card->Value = mod;
		card->Locked = false;
		card->WasLocked = false;

		if (card->Value == 1)
		{
			cardShape->Visible = false;
			suitShape->Visible = false;
			character->Visible = false;
		}

		_deck.Add(card);

		size_t row = i / 13;
		size_t column = i % 13;

		Entity* outlineEntity = static_cast<Entity*>(CurrentGame()->CreateObject("OutlineTemplate"));
		Spatial2D* outlineSpatial = outlineEntity->GetComponent<Spatial2D>();
		outlineSpatial->Position.Set(GetCardPositionX(row, column), GetCardPositionY(row, column));

		Entity* highlightEntity = static_cast<Entity*>(CurrentGame()->CreateObject("HighlightTemplate"));
		Spatial2D* highlightSpatial = highlightEntity->GetComponent<Spatial2D>();
		highlightSpatial->Position.Set(GetCardPositionX(row, column), GetCardPositionY(row, column));

		ShapeInstance* highlightShape = highlightEntity->GetComponent<ShapeInstance>();
		_highlights(row, column) = highlightShape;
	}

	_winner = CurrentGame()->World().GetObjects<Entity>().ObjectNamed("Winner")->GetComponent<TextObject>();
	_scheduler = CurrentGame()->World().GetObjects<Scheduler>().AllObjects().Last();
	_animator = CurrentGame()->World().GetObjects<PropertyAnimator>().AllObjects().Last();

	NewGame();
}

GameManager::~GameManager()
{
	for (Card* card : _cards)
		card = nullptr;

	for (size_t row = 0; row < _table.RowCount(); row++)
		for (size_t column = 0; column < _table.ColumnCount(); column++)
			_table(row, column) = nullptr;

	for (size_t row = 0; row < _highlights.RowCount(); row++)
		for (size_t column = 0; column < _highlights.ColumnCount(); column++)
			_highlights(row, column) = nullptr;

	_deck.Clear();
	_undoStack.Clear();
}

void GameManager::NewGame()
{
	_score = 52 + 48;
	_numberOfMoves = 0;

	Shuffle();
}

void GameManager::Shuffle()
{
	CurrentGame()->PlaySound("ShuffleSound", false, 1);

	_score -= (_deck.Count() - 4);

	_deck.Shuffle(_random);

	for (int i = 0; _deck.Count() > 0; i++)
	{
		size_t row = 0;
		size_t column = 0;

		for (size_t y = 0; y < _table.RowCount(); y++)
		{
			for (size_t x = 0; x < _table.ColumnCount(); x++)
			{
				if (_table(y, x) == nullptr)
				{
					row = y;
					column = x;
				}
			}
		}

		Card* card = _deck.Last();
		_deck.Remove(card);
		_table(row, column) = card;

		MoveCard(card, row, column, _moveTime);
	}

	_undoStack.Clear();

	for (size_t i = 0; i < 4; i++)
		UpdateLocked(i);

	if (CheckWin())
		Winner();

	_scheduler->Schedule(_moveTime, &GameManager::UpdateHighlights, this);
}

void GameManager::MoveCard(Card* card, size_t row, size_t column, float time)
{
	if (row < 4 && column < 13)
	{
		card->Row = row;
		card->Column = column;

		_table(row, column) = card;
	}

	Spatial2D* spatial = card->Entity()->GetComponent<Spatial2D>();
	Vector2 pos(GetCardPositionX(row, column), GetCardPositionY(row, column));
	_animator->Transition(spatial, spatial->Position, pos, time);
}

void GameManager::UpdateHighlights()
{
	ClearHighlights();

	for (int i = 0; i < 4; i++)
	{
		Card* ace = _cards[i * 13];

		if (ace->Column == 0)
		{
			// Highlight 2s
			for (unsigned int j = 0; j < 4; j++)
			{
				Card* two = _cards[(j * 13) + 1];
				HighlightCard(two->Row, two->Column);
			}

			continue;
		}

		Card* open = _table(ace->Row, ace->Column - 1);
		if (open->Value == 13 || open->Value == 1)
			continue;

		Card* highlight = GetCard(open->Value + 1, open->Suit);
		HighlightCard(highlight->Row, highlight->Column);
	}

	_isShuffling = false;
}

void GameManager::HighlightCard(size_t row, size_t column)
{
	ShapeInstance* highlight = _highlights(row, column);
	highlight->Visible = true;
}

void GameManager::ClearHighlights()
{
	for (size_t row = 0; row < _highlights.RowCount(); row++)
		for (size_t column = 0; column < _highlights.ColumnCount(); column++)
			ClearHighlight(row, column);
}

void GameManager::ClearHighlight(size_t row, size_t column)
{
	ShapeInstance* highlight = _highlights(row, column);
	highlight->Visible = false;
}

void GameManager::CardClicked(Card* selection)
{
	if (!_highlights(selection->Row, selection->Column)->Visible)
		return;

	CurrentGame()->PlaySound("CardMoveSound", false, 1);

	_numberOfMoves++;

	size_t row = selection->Row;
	size_t column = selection->Column;

	Card* ace = nullptr;
	if (selection->Value == 2)
	{
		size_t index;

		if (column == 0)
			index = row + 1;
		else
			index = 0;

		while (ace == nullptr)
		{
			if (index > 3)
				index = 0;

			ace = _table(index, 0);

			if (ace->Value != 1)
			{
				ace = nullptr;
				index++;
			}
		}
	}
	else
	{
		Card* open = GetCard(selection->Value - 1, selection->Suit);
		ace = _table(open->Row, open->Column + 1);
	}

	MoveCard(selection, ace->Row, ace->Column, _moveTime);
	MoveCard(ace, row, column, _moveTime);

	Move move;
	move.Card = selection;
	move.Row = row;
	move.Column = column;

	_undoStack.Add(move);

	ClearHighlight(ace->Row, ace->Column);
	
	_scheduler->Schedule(_moveTime, &GameManager::UpdateHighlights, this);

	if (!UpdateLocked(selection->Row))
		_score--;

	UpdateLocked(row);

	if (CheckWin())
		Winner();
}

void GameManager::ToggleSound()
{
	CurrentGame()->ToggleAudio();
}

auto GameManager::GetCard(unsigned int value, Suit suit) -> Card*
{
	Card* card = _cards[(int)suit * 13 + (value - 1)];
	return card;
}

auto GameManager::GetCardPositionX(size_t row, size_t column) -> float
{
	if (row >= 4 || column >= 13)
		return _deckX;

	return _cardStartX + ((int)column * (_cardWidth + _cardSpacing));
}

auto GameManager::GetCardPositionY(size_t row, size_t column) -> float
{
	if (row >= 4 || column >= 13)
		return _deckY;

	return _cardStartY - ((int)row * (_cardHeight + _cardSpacing));
}

auto GameManager::UpdateLocked(size_t row) -> bool
{
	int scoreTally = 0;

	for (size_t column = 0; column < _table.ColumnCount(); column++)
	{
		Card* card = _table(row, column);

		bool lock = CheckLocked(row, column);

		if (!card->WasLocked && lock)
		{
			scoreTally++;
			card->WasLocked = true;
		}

		card->Locked = lock;
	}

	if (scoreTally > 0)
		_score += static_cast<int>((5 * pow(2, scoreTally - 1)));

	return scoreTally > 0;
}

auto GameManager::CheckLocked(size_t row, size_t column) -> bool
{
	Card* card = _table(row, column);

	if (column == 0)
		return card->Value == 2;

	Card* check = _table(row, column - 1);
	return check->Locked && (check->Suit == card->Suit) && ((check->Value + 1) == card->Value);
}

void GameManager::Undo()
{
	CurrentGame()->PlaySound("ButtonSound", false, 1);

	if (_undoStack.IsEmpty())
		return;

	if (_winner->Visible == true)
		return;

	Move& move = _undoStack.Last();

	size_t row = move.Card->Row;
	size_t column = move.Card->Column;

	Card* ace = _table(move.Row, move.Column);

	CurrentGame()->PlaySound("MoveCardSound", false, 1);

	MoveCard(move.Card, ace->Row, ace->Column, _moveTime);
	MoveCard(ace, row, column, _moveTime);

	UpdateLocked(row);
	UpdateLocked(move.Card->Row);

	_scheduler->Schedule(_moveTime, &GameManager::UpdateHighlights, this);

	_undoStack.RemoveLast();
}

void GameManager::ShufflePressed()
{
	CurrentGame()->PlaySound("ButtonSound", false, 1);

	if (_isShuffling)
		return;

	ClearTable();

	CurrentGame()->PlaySound("MoveCardSound", false, 1);

	_scheduler->Schedule(_moveTime, &GameManager::Shuffle, this);
}

void GameManager::ClearTable()
{
	ClearHighlights();

	_isShuffling = true;

	for (size_t row = 0; row < _table.RowCount(); row++)
	{
		for (size_t column = 0; column < _table.ColumnCount(); column++)
		{
			Card* card = _table(row, column);

			if (card == nullptr)
				continue;

			if (!card->Locked)
			{
				_deck.Add(card);
				MoveCard(card, 15, 15, _moveTime);
				_table(row, column) = nullptr;
			}
		}
	}
}

auto GameManager::CheckWin() -> bool
{
	for (size_t row = 0; row < _table.RowCount(); row++)
	{
		for (size_t column = 0; column < _table.ColumnCount(); column++)
		{
			Card* card = _table(row, column);

			if (card->Value == 1)
				continue;

			if (!card->Locked)
				return false;
		}
	}

	return true;
}

void GameManager::Winner()
{
	_winner->Visible = true;
}

void GameManager::NewGamePressed()
{
	_winner->Visible = false;

	CurrentGame()->PlaySound("ButtonSound", false, 1);

	if (_isShuffling)
		return;

	for (Card* card : _cards)
	{
		card->Locked = false;
		card->WasLocked = false;
	}

	ClearTable();

	CurrentGame()->PlaySound("CardMoveSound", false, 1);

	_scheduler->Schedule(_moveTime, &GameManager::NewGame, this);
}
