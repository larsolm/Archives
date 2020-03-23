#pragma once

#include <Pargon.h>

using namespace Pargon;

template<typename Type, typename DataType>
class PermanentDynamicallyTypedStorage
{
public:
	auto All() const -> SequenceView<std::unique_ptr<Type>>;

	template<typename... Args> auto Create(DataType& data, Args&&... args) -> Type*;
	auto Lookup(typename Type::IdType id) -> Type*;

private:
	int _nextId = 0;
	Array<std::unique_ptr<Type>, Type::MaximumCount> _storage;
};

template<typename Type, typename DataType>
auto PermanentDynamicallyTypedStorage<Type, DataType>::All() const -> SequenceView<std::unique_ptr<Type>>
{
	return _storage.GetView(0, _nextId);
}

template<typename Type, typename DataType>
template<typename... Args>
auto PermanentDynamicallyTypedStorage<Type, DataType>::Create(DataType& data, Args&&... args) -> Type*
{
	if (_nextId < _storage.Count())
	{
		auto& object = _storage.Item(_nextId);
		object = data.Create(std::forward<Args>(args)...);
		object->Id = Type::IdType(_nextId++);
		object->Data = &data;
		return object.get();
	}

	return nullptr;
}

template<typename Type, typename DataType>
auto PermanentDynamicallyTypedStorage<Type, DataType>::Lookup(typename Type::IdType id) -> Type*
{
	return id._id < _nextId ? std::addressof(_storage.Item(id._id)) : nullptr;
}
