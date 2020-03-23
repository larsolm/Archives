#pragma once

#include <Pargon.h>

using namespace Pargon;

template<typename EnumType>
class Attributes;

template<typename EnumType>
class AttributeModifiers
{
public:
	void Augment(EnumType type);

private:
	friend class Attributes<EnumType>;

	Array<int, EnumNames<EnumType>.Count()> _counts;
};

template<typename EnumType>
class Attributes
{
public:
	auto CanIncrease(const AttributeModifiers<EnumType>& modifiers, EnumType type) const -> bool;
	auto GetAttribute(const AttributeModifiers<EnumType>& modifiers, EnumType type) const -> float;
	void FromBlueprint(BlueprintReader& reader);

private:
	Array<float, EnumNames<EnumType>.Count()> _bases;
	Array<float, EnumNames<EnumType>.Count()> _increments;
	Array<int, EnumNames<EnumType>.Count()> _maximums;
};

template<typename EnumType>
void AttributeModifiers<EnumType>::Augment(EnumType type)
{
	assert(type != EnumType::None);
	_counts.Item(static_cast<int>(type))++;
}

template<typename EnumType>
auto Attributes<EnumType>::CanIncrease(const AttributeModifiers<EnumType>& modifiers, EnumType type) const -> bool
{
	return type != EnumType::None && modifiers._counts.Item(static_cast<int>(type)) < _maximums.Item(static_cast<int>(type));
}

template<typename EnumType>
auto Attributes<EnumType>::GetAttribute(const AttributeModifiers<EnumType>& modifiers, EnumType type) const -> float
{
	assert(type != EnumType::None);
	return _bases.Item(static_cast<int>(type)) + (modifiers._counts.Item(static_cast<int>(type)) * _increments.Item(static_cast<int>(type)));
}

template<typename EnumType>
void Attributes<EnumType>::FromBlueprint(BlueprintReader& reader)
{
	if (reader.MoveDown("Bases"))
	{
		for (auto i = 0; i < EnumNames<EnumType>.Count(); i++)
			reader.ReadChild(EnumNames<EnumType>.Item(i), _bases.Item(i));
		
		reader.MoveUp();
	}

	if (reader.MoveDown("Increments"))
	{
		for (auto i = 0; i < EnumNames<EnumType>.Count(); i++)
			reader.ReadChild(EnumNames<EnumType>.Item(i), _increments.Item(i));

		reader.MoveUp();
	}

	if (reader.MoveDown("Maximums"))
	{
		for (auto i = 0; i < EnumNames<EnumType>.Count(); i++)
			if (!reader.ReadChild(EnumNames<EnumType>.Item(i), _maximums.Item(i)))
				_maximums.SetItem(i, 0);

		reader.MoveUp();
	}
}
