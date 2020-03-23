#pragma once

#include <Pargon.h>

using namespace Pargon;

template<typename Type, typename DataType>
class RemovableDynamicallyTypedStorage
{
public:
	RemovableDynamicallyTypedStorage();

	auto All() const -> SequenceView<std::unique_ptr<Type>>;

	template<typename... Args> auto Create(DataType& data, Args&&... args) -> Type*;
	auto Lookup(typename Type::IdType id) -> Type*;
	void Destroy(typename Type::IdType id);

private:
	static constexpr int LookupMask = Type::MaximumCount - 1;
	static constexpr int VersionMask = Type::MaximumCount;

	struct Entry
	{
		typename Type::IdType Id;
		int Index;
		int NextFree;
	};

	int _firstFree = 0;
	int _lastFree = Type::MaximumCount - 1;
	int _count = 0;

	Array<Entry, Type::MaximumCount> _indices;
	Array<std::unique_ptr<Type>, Type::MaximumCount> _storage;
};

template<typename Type, typename DataType>
RemovableDynamicallyTypedStorage<Type, DataType>::RemovableDynamicallyTypedStorage()
{
	for (auto i = 0; i < _storage.Count(); i++)
	{
		_indices.Item(i).Id = Type::IdType(i);
		_indices.Item(i).Index = Sequence::InvalidIndex;
		_indices.Item(i).NextFree = i + 1;
	}
}

template<typename Type, typename DataType>
auto RemovableDynamicallyTypedStorage<Type, DataType>::All() const -> SequenceView<std::unique_ptr<Type>>
{
	return _storage.GetView(0, _count);
}

template<typename Type, typename DataType>
template<typename... Args>
auto RemovableDynamicallyTypedStorage<Type, DataType>::Create(DataType& data, Args&&... args) -> Type*
{
	if (_firstFree < _indices.Count())
	{
		auto& entry = _indices.Item(_firstFree);
		entry.Id._id += VersionMask;
		entry.Index = _count++;

		auto& object = _storage.Item(entry.Index);
		object = data.Create(std::forward<Args>(args)...);
		object->Id = entry.Id;
		object->Data = &data;

		if (_firstFree == _lastFree)
			_lastFree = entry.NextFree;

		_firstFree = entry.NextFree;

		return object.get();
	}

	return nullptr;
}

template<typename Type, typename DataType>
auto RemovableDynamicallyTypedStorage<Type, DataType>::Lookup(typename Type::IdType id) -> Type*
{
	auto index = id._id & LookupMask;
	auto& entry = _indices.Item(index);

	return id._id == entry.Id._id && entry.Index != Sequence::InvalidIndex ? _storage.Item(entry.Index).get() : nullptr;
}

template<typename Type, typename DataType>
void RemovableDynamicallyTypedStorage<Type, DataType>::Destroy(typename Type::IdType id)
{
	auto index = id._id & LookupMask;
	auto& entry = _indices.Item(index);

	if (entry.Id._id == id._id)
	{
		auto& object = _storage.Item(entry.Index);

		if (entry.Index == _count - 1)
			object.reset();
		else
			object = std::move(_storage.Item(_count - 1));

		--_count;

		if (object != nullptr)
			_indices.Item(object->Id._id & LookupMask).Index = entry.Index;

		entry.Index = Sequence::InvalidIndex;

		if (_lastFree < _storage.Count())
			_indices.Item(_lastFree).NextFree = index;
		else
			_firstFree = index;

		_lastFree = index;
	}
}
