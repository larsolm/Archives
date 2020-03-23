#include "Pch.h"
#include "World/Storages/StorageId.h"

StorageId::StorageId() :
	_id(Sequence::InvalidIndex)
{
}

StorageId::StorageId(int id) :
	_id(id)
{
}
