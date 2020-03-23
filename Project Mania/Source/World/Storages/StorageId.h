#pragma once

#include <Pargon.h>

using namespace Pargon;

class StorageId
{
public:
	StorageId();

	auto Id() const -> int;
	auto IsValid() const -> bool;

protected:
	StorageId(int id);

	int _id;
};

inline
auto StorageId::Id() const -> int
{
	return _id;
}

inline
auto StorageId::IsValid() const -> bool
{
	return _id != Sequence::InvalidIndex;
}

template<typename StorageType>
class GameObjectId : private StorageId
{
private:
	friend StorageType;
	using StorageId::StorageId;
};
