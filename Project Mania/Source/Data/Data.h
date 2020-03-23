#pragma once

#include <Pargon.h>

using namespace Pargon;

struct Data
{
	String Id;
	int Index;

	String DisplayName;

	virtual void FromBlueprint(BlueprintReader& reader);
};

template<typename GroupType, typename InstanceType> auto CreateInstance() -> std::unique_ptr<GroupType>;

template<typename GroupType>
struct DataMapEntry
{
	StringView Name;
	std::unique_ptr<GroupType>(*CreateInstance)();
};

template<typename GroupType>
struct DataMap : Map<String, std::unique_ptr<GroupType>(*)()>
{
	DataMap(const std::initializer_list<DataMapEntry<GroupType>>& initializer);
	auto CreateData(StringView name) const -> std::unique_ptr<GroupType>;
};

template<typename GroupType, typename InstanceType>
auto CreateInstance() -> std::unique_ptr<GroupType>
{
	return std::make_unique<InstanceType>();
}

template<typename GroupType>
DataMap<GroupType>::DataMap(const std::initializer_list<DataMapEntry<GroupType>>& initializer)
{
	for (auto& item : initializer)
		AddOrSet(item.Name, item.CreateInstance);
}

template<typename GroupType>
auto DataMap<GroupType>::CreateData(StringView name) const -> std::unique_ptr<GroupType>
{
	auto index = GetIndex(name);
	return index == Sequence::InvalidIndex ? nullptr : ItemWithKey(name)();
}
