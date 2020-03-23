#pragma once

#include "World/Databases/Database.h"

#include <PargonShapes.h>

using namespace Pargon::Shapes;

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
class Partition
{
public:
	void Initialize(const Rectangle& bounds);

	auto Add(ObjectType* object) -> bool;
	auto Update(ObjectType* object) -> bool;
	void Remove(ObjectType* object);

	auto Find(Point2 position) -> DatabaseQuery<ObjectType>;
	auto Find(Point2 position, float radius) -> DatabaseQuery<ObjectType>;
	auto Find(const Rectangle& area) -> DatabaseQuery<ObjectType>;

private:
	struct SpatialNode
	{
		ObjectType* Object;
		SpatialNode* Previous;
		SpatialNode* Next;
		SpatialNode* NextCell;
	};

	struct Cell
	{
		SpatialNode* FirstNode;
	};

	SpatialNode* _freeNodes;
	Array<SpatialNode, MaximumEntries> _nodes;
	std::unique_ptr<Cell[]> _cells;

	Rectangle _bounds;
	Vector2 _cellSize;

	int _cellCountX;
	int _cellCountY;

	auto GetCellX(float x) -> int;
	auto GetCellY(float y) -> int;
	auto GetCell(int x, int y) -> Cell&;

	auto Reserve() -> SpatialNode*;
	void Release(SpatialNode* node);
};

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
void Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::Initialize(const Rectangle& bounds)
{
	_bounds = bounds;

	_cellSize = { static_cast<float>(CellWidth), static_cast<float>(CellHeight) };
	_cellCountX = Ceiling(_bounds.Width / _cellSize.X);
	_cellCountY = Ceiling(_bounds.Height / _cellSize.Y);
	_cells = std::make_unique<Cell[]>(_cellCountX * _cellCountY);

	for (auto i = 0; i < _nodes.LastIndex(); i++)
		_nodes.Item(i).Next = _nodes.begin() + i + 1;

	_nodes.Last().Next = nullptr;
	_freeNodes = _nodes.begin();
}

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
auto Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::Add(ObjectType* object) -> bool
{
	assert(object->Info.FirstNode == nullptr);

	auto bounds = object->GetBounds();

	if (IsColliding(bounds, _bounds))
	{
		auto minX = GetCellX(bounds.X);
		auto minY = GetCellY(bounds.Y);
		auto maxX = GetCellX(bounds.X + bounds.Width);
		auto maxY = GetCellY(bounds.Y + bounds.Height);

		SpatialNode* nextCell = nullptr;

		for (auto x = minX; x <= maxX; x++)
		{
			for (auto y = minY; y <= maxY; y++)
			{
				auto node = Reserve();
				auto& cell = GetCell(x, y);

				node->Object = object;
				node->Previous = nullptr;
				node->Next = cell.FirstNode;
				node->NextCell = nextCell;

				if (cell.FirstNode != nullptr)
					cell.FirstNode->Previous = node;

				cell.FirstEntry = node;

				nextCell = node;
			}
		}

		object->Info.FirstNode = nextCell;

		return true;
	}

	return false;
}

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
auto Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::Update(ObjectType* object) -> bool
{
	Remove(object);
	return Add(object);
}

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
void Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::Remove(ObjectType* object)
{
	assert(object->Info.FirstNode != nullptr);

	auto node = object->Info.FirstNode;
	auto first = node;

	while (first != nullptr)
	{
		for (auto x = 0u; x < _cellCountX; x++)
		{
			for (auto y = 0u; y < _cellCountY; y++)
			{
				auto& cell = GetCell(x, y);
				if (cell.FirstNode == first)
					cell.FirstNode = first->Next;
			}
		}

		first = first->NextCell;
	}

	object->Info.FirstNode = nullptr;

	while (node != nullptr)
	{
		if (node->Previous != nullptr) node->Previous->Next = node->Next;
		if (node->Next != nullptr) node->Next->Previous = node->Previous;

		auto next = node->NextCell;
		Release(node);
		node = next;
	}
}

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
auto Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::Find(Point2 position) -> DatabaseQuery<ObjectType>
{
	DatabaseQuery<Type> result;

	auto x = GetCellX(position.X);
	auto y = GetCellY(position.Y);
	auto count = 0u;

	auto& cell = GetCell(x, y);

	for (auto node = cell.FirstNode; node != nullptr; node = node->Next)
	{
		auto bounds = node->Object->GetBody().GetBounds();
		if (IsContained(position, bounds))
			result.Add(node->Object);
	}

	return result;
}

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
auto Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::Find(Point2 position, float radius) -> DatabaseQuery<ObjectType>
{
	DatabaseQuery<ObjectType> result;

	auto radiusSquared = radius * radius;
	auto minX = GetCellX(position.X - radius);
	auto minY = GetCellY(position.Y - radius);
	auto maxX = GetCellX(position.X + radius);
	auto maxY = GetCellY(position.Y + radius);
	auto count = 0u;

	for (auto x = minX; x <= maxX; x++)
	{
		for (auto y = minY; y <= maxY; y++)
		{
			auto& cell = GetCell(x, y);

			for (auto node = cell.FirstNode; node != nullptr; node = node->Next)
			{
				auto point = GetClosestPoint(position, node->Object->GetBounds());
				auto distance = (point - position).GetLengthSquared();
				if (distance < radiusSquared)
					result.Add(node->Object);
			}
		}
	}

	return result;
}

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
auto Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::Find(const Rectangle& area) -> DatabaseQuery<ObjectType>
{
	DatabaseQuery<ObjectType> result;

	auto minX = GetCellX(area.X);
	auto minY = GetCellY(area.Y);
	auto maxX = GetCellX(area.X + area.Width);
	auto maxY = GetCellY(area.Y + area.Height);

	for (auto x = minX; x <= maxX; x++)
	{
		for (auto y = minY; y <= maxY; y++)
		{
			auto& cell = GetCell(x, y);

			for (auto node = cell.FirstNode; node != nullptr; node = node->Next)
			{
				if (IsColliding(node->Object->GetBounds(), area))
					result.Add(node->Object);
			}
		}
	}

	return result;
}

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
auto Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::GetCellX(float x) -> int
{
	auto cell = Maximum((x - _bounds.X) / _cellSize.X, 0.0f);
	return Minimum(static_cast<int>(cell), _cellCountX - 1);
}

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
auto Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::GetCellY(float y) -> int
{
	auto cell = Maximum((y - _bounds.Y) / _cellSize.Y, 0.0f);
	return Minimum(static_cast<int>(cell), _cellCountY - 1);
}

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
auto Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::GetCell(int x, int y) -> Cell&
{
	return _cells[y * _cellCountX + x];
}

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
auto Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::Reserve() -> SpatialNode*
{
	auto node = _freeNodes;

	if (_freeNodes != nullptr)
		_freeNodes = _freeNodes->Next;

	return node;
}

template<typename ObjectType, int CellWidth, int CellHeight, int MaximumEntries>
void Partition<ObjectType, CellWidth, CellHeight, MaximumEntries>::Release(SpatialNode* node)
{
	node->Next = _freeNodes;
	_freeNodes = node;
}
