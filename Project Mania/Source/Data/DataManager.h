#pragma once

#include "Data/AugmentData.h"
#include "Data/CharacterData.h"
#include "Data/ChassisData.h"
#include "Data/ComponentData.h"
#include "Data/EquipmentData.h"
#include "Data/ItemData.h"
#include "Data/PartData.h"
#include "Data/SkeletonData.h"
#include "Data/UpgradeData.h"
#include "Data/WeaponData.h"

#include <Pargon.h>

using namespace Pargon;

class DataManager
{
public:
	void Load();

	auto AugmentDatas() const -> SequenceView<std::unique_ptr<AugmentData>>;
	auto CharacterDatas() const -> SequenceView<std::unique_ptr<CharacterData>>;
	auto ChassisDatas() const -> SequenceView<std::unique_ptr<ChassisData>>;
	auto ComponentDatas() const -> SequenceView<std::unique_ptr<ComponentData>>;
	auto EquipmentDatas() const -> SequenceView<std::unique_ptr<EquipmentData>>;
	auto ItemDatas() const -> SequenceView<std::unique_ptr<ItemData>>;
	auto SkeletonDatas() const -> SequenceView<std::unique_ptr<SkeletonData>>;
	auto UpgradeDatas() const -> SequenceView<std::unique_ptr<UpgradeData>>;
	auto WeaponDatas() const -> SequenceView<std::unique_ptr<WeaponData>>;

	auto GetAugmentData(int index) const -> AugmentData*;
	auto GetCharacterData(int index) const -> CharacterData*;
	auto GetChassisData(int index) const -> ChassisData*;
	auto GetComponentData(int index) const -> ComponentData*;
	auto GetEquipmentData(int index) const -> EquipmentData*;
	auto GetItemData(int index) const -> ItemData*;
	auto GetSkeletonData(int index) const -> SkeletonData*;
	auto GetUpgradeData(int index) const -> UpgradeData*;
	auto GetWeaponData(int index) const -> WeaponData*;

	auto GetAugmentData(StringView id) const -> AugmentData*;
	auto GetCharacterData(StringView id) const -> CharacterData*;
	auto GetChassisData(StringView id) const -> ChassisData*;
	auto GetComponentData(StringView id) const -> ComponentData*;
	auto GetEquipmentData(StringView id) const -> EquipmentData*;
	auto GetItemData(StringView id) const -> ItemData*;
	auto GetSkeletonData(StringView id) const -> SkeletonData*;
	auto GetUpgradeData(StringView id) const -> UpgradeData*;
	auto GetWeaponData(StringView id) const -> WeaponData*;

private:
	Map<String, std::unique_ptr<AugmentData>> _augments;
	Map<String, std::unique_ptr<ChassisData>> _chasses;
	Map<String, std::unique_ptr<CharacterData>> _characters;
	Map<String, std::unique_ptr<ComponentData>> _components;
	Map<String, std::unique_ptr<EquipmentData>> _equipment;
	Map<String, std::unique_ptr<ItemData>> _items;
	Map<String, std::unique_ptr<SkeletonData>> _skeletons;
	Map<String, std::unique_ptr<UpgradeData>> _upgrades;
	Map<String, std::unique_ptr<WeaponData>> _weapons;
};
