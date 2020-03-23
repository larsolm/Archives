#pragma once

#include <Pargon.h>

using namespace Pargon;

template<typename Type>
class DatabaseIterator
{
public:
	auto operator==(const DatabaseIterator<Type>& other) const -> bool;
	auto operator!=(const DatabaseIterator<Type>& other) const -> bool;
	auto operator++() -> DatabaseIterator<Type>&;
	auto operator++(int) -> DatabaseIterator<Type>;
	auto operator*() -> Type*;
	auto operator->() -> Type*;

private:
	friend class DatabaseQuery<Type>;

	DatabaseIterator(Type* object);

	Type* _object;
};

template<typename Type>
class DatabaseQuery
{
public:
	auto begin() const -> DatabaseIterator<Type>;
	auto end() const -> DatabaseIterator<Type>;

	auto Count() const -> unsigned int;
	auto ItemAt(unsigned int count) const -> Type*;
	auto AsList() const -> List<Type*>;
	auto Contains(Type* object) const -> bool;
	auto IndexOf(Type* object) const -> unsigned int;

	void Add(Type* object);

private:
	Type* _firstResult = nullptr;
	unsigned int _count = 0;
};

template<typename Type>
struct DatabaseEntry
{
	Type* Object;
	DatabaseEntry<Type>* Previous;
	DatabaseEntry<Type>* Next;
	DatabaseEntry<Type>* NextCell;
};

template<typename Type>
struct RaycastQuery
{
	Type* Object;
	Vector2 Position;
	float Time;
};

template<typename Type>
struct DatabaseInfo
{
private:
	friend class DatabaseIterator<Type>;
	friend class DatabaseQuery<Type>;
	template<typename Type> friend class BruteForce;
	template<typename Type, unsigned int CellWidth, unsigned int CellHeight, unsigned int MaximumEntries> friend class Partition;

	DatabaseEntry<Type>* FirstCell = nullptr;
	Type* NextResult = nullptr;
};

template<typename Type>
auto DatabaseIterator<Type>::operator==(const DatabaseIterator<Type>& other) const -> bool
{
	return _object == other._object;
}

template<typename Type>
auto DatabaseIterator<Type>::operator!=(const DatabaseIterator<Type>& other) const -> bool
{
	return _object != other._object;
}

template<typename Type>
auto DatabaseIterator<Type>::operator++() -> DatabaseIterator<Type>&
{
	_object = _object->Info.NextResult;
	return *this;
}

template<typename Type>
auto DatabaseIterator<Type>::operator++(int) -> DatabaseIterator<Type>
{
	auto iterator = *this;
	operator++();
	return iterator;
}

template<typename Type>
auto DatabaseIterator<Type>::operator*() -> Type*
{
	return _object;
}

template<typename Type>
auto DatabaseIterator<Type>::operator->() -> Type*
{
	return _object;
}

template<typename Type>
DatabaseIterator<Type>::DatabaseIterator(Type* object) :
	_object(object)
{
}

template<typename Type>
auto DatabaseQuery<Type>::begin() const -> DatabaseIterator<Type>
{
	return { _firstResult };
}

template<typename Type>
auto DatabaseQuery<Type>::end() const -> DatabaseIterator<Type>
{
	return { nullptr };
}

template<typename Type>
auto DatabaseQuery<Type>::Count() const -> unsigned int
{
	return _count;
}

template<typename Type>
auto DatabaseQuery<Type>::ItemAt(unsigned int count) const -> Type*
{
	if (count >= _count)
		return nullptr;

	auto object = _firstResult;

	for (auto i = 0u; i < count; i++)
		object = object->Info.NextResult;

	return object;
}

template<typename Type>
auto DatabaseQuery<Type>::AsList() const -> List<Type*>
{
	List<Type*> list;

	for (auto result = _firstResult; result != nullptr; result = result->Info.NextResult)
		list.Add(result);

	return list;
}

template<typename Type>
auto DatabaseQuery<Type>::Contains(Type* object) const -> bool
{
	for (auto result = _firstResult; result != nullptr; result = result->Info.NextResult)
	{
		if (result == object)
			return true;
	}

	return false;
}

template<typename Type>
auto DatabaseQuery<Type>::IndexOf(Type* object) const -> unsigned int
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

template<typename Type>
void DatabaseQuery<Type>::Add(Type* object)
{
	if (!Contains(object))
	{
		object->Info.NextResult = _firstResult;
		_firstResult = object;
		_count++;
	}
}
