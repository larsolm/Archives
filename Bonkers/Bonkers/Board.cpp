#include "Bonkers/Bonkers.h"

using namespace Bonkers;
using namespace Pargon;

auto Board::Geometry() -> Pargon::Graphics::Geometry&
{
	return _geometry;
}

void Board::InitializeBoard()
{
	GenerateBoard();
	GenerateGeometry();
}

void Board::GenerateBoard()
{
	_spaces.SetCount(_numberOfSpaces);
	_spaces[0].MakeScore();

	for (auto i : _scoreLocations)
		_spaces[i].MakeScore();

	for (auto i : _loseLocations)
		_spaces[i].MakeLose();
}

auto Board::TotalSpaces() const -> const size_t
{
	return _numberOfSpaces;
}

auto Board::GetNextScore(size_t index) const -> const size_t
{
	for (auto i = index + 1; i < _spaces.Count(); i++)
	{
		if (_spaces[i].Type() == SpaceType::Score)
			return i - index;
	}

	return _spaces.Count() - index;
}

void Board::ClearHit()
{
	for (auto& space : _spaces)
		space.SetHit(false);
}

void Board::PlaceTile(Tile* tile, size_t index)
{
	auto position = _spaces[index].Position();
	auto rotation = _spaces[index].Rotation();
	auto entity = dynamic_cast<Pargon::Ensemble*>(Pargon::CurrentGame()->CreateObject(_tileTemplate));
	auto spatial = entity->GetComponent<Pargon::Spatial2D>();
	
	spatial->Position = position;
	spatial->Rotation = rotation;
	spatial->NaturalOffset.Set(0, 0);
	spatial->NaturalSize.Set(1, 1);

	auto text = entity->FindComponent<Pargon::Text::TextObject>();
	text->Characters = tile->GetText();

	auto shape = entity->FindComponent<Pargon::Shapes::ShapeInstance>();
	shape->Texture = tile->Icon();

	_spaces[index].PlaceTile(tile);
}

auto Board::GetSpacePosition(size_t index) const -> Vector2
{
	return _spaces[index].Position();
}

auto Board::GetSpace(size_t index) -> Space&
{
	return _spaces[index];
}

auto CubicBezierPath::CalculateLength() -> float
{
	auto length = 0.0f;

	for (auto i = size_t{ 0 }; i < Nodes.Count(); i++)
	{
		auto n = i == Nodes.Count() - 1 ? 0 : i + 1;

		auto& start = Nodes[i];
		auto& end = Nodes[n];

		auto& a = start.Position;
		auto startRotation = Pargon::Trigonometry::ToRadians(start.Rotation);
		auto b = a + (Pargon::Vector2(cos(startRotation), sin(startRotation)) * start.Weight);

		auto& d = end.Position;
		auto endRotation = Pargon::Trigonometry::ToRadians(end.Rotation);
		auto c = d - (Pargon::Vector2(cos(endRotation), sin(endRotation)) * end.Weight);

		auto one = Pargon::Vector2();
		auto two = a;

		for (auto t = 0.0f; t < 1.001f; t += 0.001f)
		{
			auto it = 1.0f - t;
			one = two;
			two = (a * it * it * it) + (b * it * it * t * 3.0f) + (c * it * t * t * 3.0f) + (d * t * t * t);
			length += (two - one).Length();
		}
	}

	return length;
}

auto CubicBezierPath::CalculatePosition(float advance) -> Pargon::Vector2
{
	auto length = 0.0f;

	for (auto i = size_t{ 0 }; i < Nodes.Count(); i++)
	{
		auto n = i == Nodes.Count() - 1 ? 0 : i + 1;

		auto& start = Nodes[i];
		auto& end = Nodes[n];

		auto& a = start.Position;
		auto startRotation = Pargon::Trigonometry::ToRadians(start.Rotation);
		auto b = a + (Pargon::Vector2(cos(startRotation), sin(startRotation)) * start.Weight);

		auto& d = end.Position;
		auto endRotation = Pargon::Trigonometry::ToRadians(end.Rotation);
		auto c = d - (Pargon::Vector2(cos(endRotation), sin(endRotation)) * end.Weight);

		auto one = Pargon::Vector2();
		auto two = a;

		for (auto t = 0.0f; t < 1.001f; t += 0.001f)
		{
			auto it = 1.0f - t;
			one = two;
			two = (a * it * it * it) + (b * it * it * t * 3.0f) + (c * it * t * t * 3.0f) + (d * t * t * t);
			length += (two - one).Length();

			if (length > advance)
				return one;
		}
	}

	return Pargon::Vector2(0.0f, 0.0f);
}

namespace
{
	struct BoardVertex
	{
		float X, Y;
		Pargon::Color Color;
	};

	void AddRectangle(const Pargon::Vector2& top1, const Pargon::Vector2& bottom1, const Pargon::Vector2& top2, const Pargon::Vector2& bottom2, const Pargon::Color& color, Pargon::List<BoardVertex>& vertices)
	{
		vertices.Add(BoardVertex{ top1.X, top1.Y, color });
		vertices.Add(BoardVertex{ bottom1.X, bottom1.Y, color });
		vertices.Add(BoardVertex{ top2.X, top2.Y, color });
		vertices.Add(BoardVertex{ top2.X, top2.Y, color });
		vertices.Add(BoardVertex{ bottom1.X, bottom1.Y, color });
		vertices.Add(BoardVertex{ bottom2.X, bottom2.Y, color });
	}

	void AddRectangle(const Pargon::Vector2& start, const Pargon::Vector2& end, const Pargon::Color& color, float width, Pargon::List<BoardVertex>& vertices)
	{
		auto vector = (end - start).NormalizeTo(width);
		auto perpendicular = Pargon::Vector2(vector.Y, -vector.X);

		auto top1 = start + perpendicular;
		auto bottom1 = start - perpendicular;
		auto top2 = end + perpendicular;
		auto bottom2 = end - perpendicular;

		AddRectangle(top1, bottom1, top2, bottom2, color, vertices);
	}

	auto GetColor(SpaceType type) -> Pargon::Color
	{
		switch (type)
		{
			case SpaceType::Empty:
			case SpaceType::Tiled:
				return Pargon::Color::White();
			case SpaceType::Score:
				return Pargon::Color::Green();
			case SpaceType::Lose:
				return Pargon::Color::Purple();
		}

		return Pargon::Color::Black();
	}
}

void Board::GenerateGeometry()
{
	Pargon::List<BoardVertex> vertices;

	auto length = _shape.CalculateLength();
	auto advance = length / _spaces.Count();
	auto position = 0.0f;

	auto firstStart = _shape.CalculatePosition(length - advance);
	auto firstEnd = _shape.CalculatePosition(0.0f);
	auto firstVector = (firstEnd - firstStart).Normalize();
	auto firstPerpendicular = Pargon::Vector2(firstVector.Y, -firstVector.X) * 20.0f;

	auto previousPerpendicular = firstPerpendicular;

	for (auto i = size_t{ 0 }; i < _spaces.Count() - 1; i++)
	{
		auto start = _shape.CalculatePosition(position);
		auto end = _shape.CalculatePosition(position + advance);
		auto vector = (end - start).Normalize();
		auto perpendicular = Pargon::Vector2(vector.Y, -vector.X) * 20.0f;
		auto color = GetColor(_spaces[i].Type());

		AddRectangle(start + previousPerpendicular, start - previousPerpendicular, end + perpendicular, end - perpendicular, color, vertices);
		AddRectangle(start + previousPerpendicular, end + perpendicular, Pargon::Color::Black(), 2.0f, vertices);
		AddRectangle(start + previousPerpendicular, start - previousPerpendicular, Pargon::Color::Black(), 2.0f, vertices);
		AddRectangle(end - perpendicular, end + perpendicular, Pargon::Color::Black(), 2.0f, vertices);
		AddRectangle(end - perpendicular, start - previousPerpendicular, Pargon::Color::Black(), 2.0f, vertices);

		_spaces[i].SetPosition(start + ((end - start) * 0.5f));
		_spaces[i].SetRotation(Pargon::Trigonometry::ToDegrees(atan2(vector.Y, vector.X)));

		previousPerpendicular = perpendicular;
		position += advance;
	}

	AddRectangle(firstStart + previousPerpendicular, firstStart - previousPerpendicular, firstEnd + firstPerpendicular, firstEnd - firstPerpendicular, GetColor(_spaces.Last().Type()), vertices);
	AddRectangle(firstStart + previousPerpendicular, firstEnd + firstPerpendicular, Pargon::Color::Black(), 2.0f, vertices);
	AddRectangle(firstStart + previousPerpendicular, firstStart - previousPerpendicular, Pargon::Color::Black(), 2.0f, vertices);
	AddRectangle(firstEnd - firstPerpendicular, firstEnd + firstPerpendicular, Pargon::Color::Black(), 2.0f, vertices);
	AddRectangle(firstEnd - firstPerpendicular, firstStart - previousPerpendicular, Pargon::Color::Black(), 2.0f, vertices);

	_spaces.Last().SetPosition(firstStart + ((firstEnd - firstStart) * 0.5f));
	_spaces.Last().SetRotation(Pargon::Trigonometry::ToDegrees(atan2(firstVector.Y, firstVector.X)));

	_geometry.VertexLayout =
	{
		Pargon::Graphics::ShaderElement{ "position", Pargon::Graphics::ShaderElementType::Vector2, Pargon::Graphics::ShaderElementUsage::Position },
		Pargon::Graphics::ShaderElement{ "color", Pargon::Graphics::ShaderElementType::Color, Pargon::Graphics::ShaderElementUsage::Color }
	};

	auto& content = _geometry.Data().ManualLoad("Board Geometry");
	content.Strip = false;
	content.VertexCount = vertices.Count();
	content.VertexSize = sizeof(BoardVertex);
	content.Vertices = std::make_unique<char[]>(content.VertexCount * content.VertexSize);

	std::copy(vertices.begin(), vertices.end(), reinterpret_cast<BoardVertex*>(content.Vertices.get()));
}

template<>
void Pargon::CreateMembers<CubicBezierPathNode>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<CubicBezierPathNode>();

	type->AddField("Position", &CubicBezierPathNode::Position);
	type->AddField("Rotation", &CubicBezierPathNode::Rotation);
	type->AddField("Weight", &CubicBezierPathNode::Weight);
}

template<>
void Pargon::CreateMembers<CubicBezierPath>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<CubicBezierPath>();

	type->AddField("Nodes", &CubicBezierPath::Nodes);
}

template<>
void Pargon::CreateMembers<Board>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<Board>();

	type->AddField("Shape", &Board::_shape);
	type->AddField("NumberOfSpaces", &Board::_numberOfSpaces);
	type->AddField("ScoreLocations", &Board::_scoreLocations);
	type->AddField("LoseLocations", &Board::_loseLocations);
	type->AddField("TileTemplate", &Board::_tileTemplate);
}
