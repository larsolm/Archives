#pragma once

#include <Pargon.h>

using namespace Pargon;

template<typename Type>
class PermanentStaticallyTypedStorage
{
public:
	auto All() const -> SequenceView<std::unique_ptr<Type>>;

	template<typename... Args> auto Create(Args&&... args) -> Type*;
	auto Lookup(typename Type::IdType id) -> Type*;

private:
	int _nextId = 0;
	Array<std::unique_ptr<Type>, Type::MaximumCount> _storage;
};

template<typename Type>
auto PermanentStaticallyTypedStorage<Type>::All() const -> SequenceView<std::unique_ptr<Type>>
{
	return _storage.GetView(0, _nextId);
}

template<typename Type>
template<typename... Args>
auto PermanentStaticallyTypedStorage<Type>::Create(Args&&... args) -> Type*
{
	if (_nextId < _storage.Count())
	{
		auto& object = _storage.Item(_nextId);
		object = std::make_unique<Type>(std::forward<Args>(args)...);
		object->Id = Type::IdType(_nextId++);
		return object.get();
	}

	return nullptr;
}

template<typename Type>
auto PermanentStaticallyTypedStorage<Type>::Lookup(typename Type::IdType id) -> Type*
{
	return id.Id() < _nextId ? std::addressof(_storage.Item(id.Id())) : nullptr;
}
