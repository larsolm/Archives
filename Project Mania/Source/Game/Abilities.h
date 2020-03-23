#pragma once

#include <Pargon.h>

using namespace Pargon;

template<typename EnumType>
class Abilities
{
public:
	Abilities();

	auto HasAbility(EnumType ability) const -> bool;
	auto GetAbilityStrength(EnumType ability) const -> int;
	void SetAbilityStrength(EnumType ability, int stregth);
	void AddAbility(EnumType ability, int strength = 1);
	void RemoveAbility(EnumType ability, int strength = 1);
	void AddAbilities(const Abilities& abilities);
	void RemoveAbilities(const Abilities& abilities);

	void ToBlueprint(BlueprintWriter& writer) const;
	void FromBlueprint(BlueprintReader& reader);

private:
	Array<int, EnumNames<EnumType>.Count()> _abilities;
};

template<typename EnumType>
Abilities<EnumType>::Abilities()
{
	for (auto i = 0; i < EnumNames<EnumType>.Count(); i++)
		_abilities.SetItem(i, 0);
}

template<typename EnumType>
auto Abilities<EnumType>::HasAbility(EnumType ability) const -> bool
{
	return _abilities.Item(static_cast<int>(ability)) > 0;
}

template<typename EnumType>
auto Abilities<EnumType>::GetAbilityStrength(EnumType ability) const -> int
{
	return _abilities.Item(static_cast<int>(ability));
}

template<typename EnumType>
void Abilities<EnumType>::SetAbilityStrength(EnumType ability, int stregth)
{
	_abilities.SetItem(static_cast<int>(ability), stregth);
}

template<typename EnumType>
void Abilities<EnumType>::AddAbility(EnumType ability, int strength)
{
	_abilities.Item(static_cast<int>(ability)) += strength;
}

template<typename EnumType>
void Abilities<EnumType>::RemoveAbility(EnumType ability, int strength)
{
	_abilities.Item(static_cast<int>(ability)) -= strength;
}

template<typename EnumType>
void Abilities<EnumType>::AddAbilities(const Abilities& abilities)
{
	for (auto i = 0; i < EnumNames<EnumType>.Count(); i++)
		AddAbility(static_cast<EnumType>(i), abilities._abilities.Item(i));
}

template<typename EnumType>
void Abilities<EnumType>::RemoveAbilities(const Abilities& abilities)
{
	for (auto i = 0; i < EnumNames<EnumType>.Count(); i++)
		RemoveAbility(static_cast<EnumType>(i), abilities._abilities.Item(i));
}

template<typename EnumType>
void Abilities<EnumType>::ToBlueprint(BlueprintWriter& writer) const
{
	for (auto i = 0; i < EnumNames<EnumType>.Count(); i++)
	{
		if (HasAbility(static_cast<EnumType>(i)))
			writer.Write(EnumNames<EnumType>.Item(i));
	}
}

template<typename EnumType>
void Abilities<EnumType>::FromBlueprint(BlueprintReader& reader)
{
	for (auto i = 0; i < EnumNames<EnumType>.Count(); i++)
	{
		auto name = EnumNames<EnumType>.Item(i);
		auto strength = 0;
		reader.ReadChild(name, count);

		AddAbility(static_cast<EnumType>(i), strength);
	}
}
