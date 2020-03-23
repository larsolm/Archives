#pragma once

#include "World/Databases/Database.h"

#include <PargonShapes.h>

using namespace Pargon::Shapes;

template<typename ObjectType>
class BruteForce
{
public:
	void Initialize(const Rectangle& bounds);

	auto Add(ObjectType* object) -> bool;
	void Remove(ObjectType* object);

	auto Find(Point2 position) -> DatabaseQuery<ObjectType>;
	auto Find(Point2 position, float radius) -> DatabaseQuery<ObjectType>;
	auto Find(const Rectangle& area) -> DatabaseQuery<ObjectType>;

private:
	friend class DatabaseInfo<BruteForce, ObjectType>;

	struct SpatialNode
	{
		ObjectType* Object;
		SpatialNode* Previous;
		SpatialNode* Next;
	};

	Rectangle _bounds;

	SpatialNode* _freeNodes;
	SpatialNode* _firstNode;
	Array<SpatialNode, ObjectType::MaximumCount> _nodes;

	auto Reserve() -> SpatialNode*;
	void Release(SpatialNode* node);
};

template<typename ObjectType>
void BruteForce<ObjectType>::Initialize(const Rectangle& bounds)
{
	_bounds = bounds;

	_nodes.First().Previous = nullptr;
	_nodes.First().Next = _entries.begin() + 1;

	for (auto i = 1; i < _nodes.LastIndex(); i++)
	{
		_nodes.Item(i).Previous = _nodes.begin() + i - 1;
		_nodes.Item(i).Next = _nodes.begin() + i + 1;
	}

	_nodes.Last().Previous = _nodes.begin() + _nodes.Count() - 2;
	_nodes.Last().Next = nullptr;

	_freeNodes = _nodes.begin();
	_firstNode = nullptr;
}

template<typename ObjectType>
auto BruteForce<ObjectType>::Add(ObjectType* object) -> bool
{
	assert(object->Info.FirstNode == nullptr);

	auto bounds = object->GetBounds();
	auto node = Reserve();
	if (node != nullptr && IsColliding(bounds, _bounds))
	{
		node->Object = object;
		node->Previous = nullptr;
		node->Next = _firstNode;

		if (_firstNode != nullptr)
			_firstNode->Previous = node;

		_firstNode = node;

		object->Info.FirstNode = node;
		return true;
	}
		
	return false;
}

template<typename ObjectType>
void BruteForce<ObjectType>::Remove(ObjectType* object)
{
	assert(object->Info.FirstNode != nullptr);

	auto node = object->Info.FirstNode;

	if (_firstEntry == entry)
		_firstEntry = entry->Next;

	if (node->Previous != nullptr) node->Previous->Next = node->Next;
	if (node->Next != nullptr) node->Next->Previous = node->Previous;

	Release(node);

	object->Info.FirstNode = nullptr;
}

template<typename ObjectType>
auto BruteForce<ObjectType>::Find(Point2 position) -> DatabaseQuery<ObjectType>
{
	DatabaseQuery<ObjectType> result;

	for (auto node = _firstNode; node != nullptr; node = node->Next)
	{
		if (IsContained(position, node->Object->GetBounds()))
			result.Add(node->Object);
	}

	return result;
}

template<typename ObjectType>
auto BruteForce<ObjectType>::Find(Point2 position, float radius) -> DatabaseQuery<ObjectType>
{
	DatabaseQuery<ObjectType> result;

	auto radiusSquared = radius * radius;

	for (auto node = _firstNode; node != nullptr; node = node->Next)
	{
		auto point = GetClosestPoint(position, node->Object->GetBounds());
		auto distance = (point - position).GetLengthSquared();
		if (distance < radiusSquared)
			result.Add(node->Object);
	}

	return result;
}

template<typename ObjectType>
auto BruteForce<ObjectType>::Find(const Rectangle& area) -> DatabaseQuery<ObjectType>
{
	DatabaseQuery<ObjectType> result;

	for (auto node = _firstNode; node != nullptr; node = node->Next)
	{
		if (IsColliding(node->Object->GetBounds(), area))
			result.Add(node->Object);
	}

	return result;
}

template<typename ObjectType>
auto BruteForce<ObjectType>::Reserve() -> SpatialNode*
{
	auto entry = _freeNodes;

	if (_freeNodes != nullptr)
		_freeNodes = _freeNodes->Next;

	return entry;
}

template<typename ObjectType>
void BruteForce<ObjectType>::Release(SpatialNode* node)
{
	node->Next = _freeEntries;
	_freeEntries = node;
}
