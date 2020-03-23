#pragma once

#include <Pargon.h>

using namespace Pargon;

template<typename Type> class DatabaseQuery;

template<typename ObjectType>
class DatabaseIterator
{
public:
	auto operator==(const DatabaseIterator<ObjectType>& other) const -> bool;
	auto operator!=(const DatabaseIterator<ObjectType>& other) const -> bool;
	auto operator++() -> DatabaseIterator<ObjectType>&;
	auto operator++(int) -> DatabaseIterator<ObjectType>;
	auto operator*() -> ObjectType*;
	auto operator->() -> ObjectType*;

private:
	friend class DatabaseQuery<ObjectType>;

	DatabaseIterator(ObjectType* object);

	ObjectType* _object;
};

template<typename ObjectType>
class DatabaseQuery
{
public:
	auto begin() const -> DatabaseIterator<ObjectType>;
	auto end() const -> DatabaseIterator<ObjectType>;

	auto Count() const -> unsigned int;
	auto ItemAt(unsigned int count) const -> ObjectType*;
	auto AsList() const -> List<ObjectType*>;
	auto Contains(ObjectType* object) const -> bool;
	auto IndexOf(ObjectType* object) const -> unsigned int;

	void Add(ObjectType* object);

private:
	ObjectType* _firstResult = nullptr;
	unsigned int _count = 0;
};

template<typename DatabaseType, typename ObjectType>
struct DatabaseInfo
{
private:
	friend class DatabaseIterator<ObjectType>;
	friend class DatabaseQuery<ObjectType>;
	friend class DatabaseType;
	friend class DatabaseType::SpatialNode;

	DatabaseType::SpatialNode* FirstNode = nullptr;
	ObjectType* NextResult = nullptr;
};

template<typename ObjectType>
auto DatabaseIterator<ObjectType>::operator==(const DatabaseIterator<ObjectType>& other) const -> bool
{
	return _object == other._object;
}

template<typename ObjectType>
auto DatabaseIterator<ObjectType>::operator!=(const DatabaseIterator<ObjectType>& other) const -> bool
{
	return _object != other._object;
}

template<typename ObjectType>
auto DatabaseIterator<ObjectType>::operator++() -> DatabaseIterator<ObjectType>&
{
	_object = _object->Info.NextResult;
	return *this;
}

template<typename ObjectType>
auto DatabaseIterator<ObjectType>::operator++(int) -> DatabaseIterator<ObjectType>
{
	auto iterator = *this;
	operator++();
	return iterator;
}

template<typename ObjectType>
auto DatabaseIterator<ObjectType>::operator*() -> ObjectType*
{
	return _object;
}

template<typename ObjectType>
auto DatabaseIterator<ObjectType>::operator->() -> ObjectType*
{
	return _object;
}

template<typename ObjectType>
DatabaseIterator<ObjectType>::DatabaseIterator(ObjectType* object) :
	_object(object)
{
}

template<typename ObjectType>
auto DatabaseQuery<ObjectType>::begin() const -> DatabaseIterator<ObjectType>
{
	return { _firstResult };
}

template<typename ObjectType>
auto DatabaseQuery<ObjectType>::end() const -> DatabaseIterator<ObjectType>
{
	return { nullptr };
}

template<typename ObjectType>
auto DatabaseQuery<ObjectType>::Count() const -> unsigned int
{
	return _count;
}

template<typename ObjectType>
auto DatabaseQuery<ObjectType>::ItemAt(unsigned int count) const -> ObjectType*
{
	if (count >= _count)
		return nullptr;

	auto object = _firstResult;

	for (auto i = 0u; i < count; i++)
		object = object->Info.NextResult;

	return object;
}

template<typename ObjectType>
auto DatabaseQuery<ObjectType>::AsList() const -> List<ObjectType*>
{
	List<Type*> list;

	for (auto result = _firstResult; result != nullptr; result = result->Info.NextResult)
		list.Add(result);

	return list;
}

template<typename ObjectType>
auto DatabaseQuery<ObjectType>::Contains(ObjectType* object) const -> bool
{
	for (auto result = _firstResult; result != nullptr; result = result->Info.NextResult)
	{
		if (result == object)
			return true;
	}

	return false;
}

template<typename ObjectType>
auto DatabaseQuery<ObjectType>::IndexOf(ObjectType* object) const -> unsigned int
{
	auto index = 0u;

	for (auto result = _firstResult; result != nullptr; result = result->Info.NextResult)
	{
		if (result == object)
			return index;

		index++;
	}

	return Sequence::InvalidIndex;
}

template<typename ObjectType>
void DatabaseQuery<ObjectType>::Add(ObjectType* object)
{
	if (!Contains(object))
	{
		object->Info.NextResult = _firstResult;
		_firstResult = object;
		_count++;
	}
}
