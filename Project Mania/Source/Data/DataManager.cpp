#include "Pch.h"
#include "Data/DataManager.h"

namespace
{
	auto ReadFile(const File& file, Blueprint& blueprint) -> bool
	{
		if (!file.GetExists())
		{
			Log().Write("File does not exist: {}", FormatString("{}/{}.{}", file.GetDirectory().Path(), file.GetName(), file.GetExtension()));
			return false;
		}

		auto json = file.ReadText().Text;
		auto reader = StringReader(json);

		reader.Read(blueprint, "json");
		if (reader.HasFailed())
		{
			Log().Write("Failed to read JSON data for {}", FormatString("{}/{}.{}", file.GetDirectory().Path(), file.GetName(), file.GetExtension()));
			return false;
		}

		return true;
	}

	template<typename DataType>
	void ReadData(StringView id, Blueprint& blueprint, std::unique_ptr<DataType> data, Map<String, std::unique_ptr<DataType>>& datas)
	{
		auto reader = BlueprintReader(blueprint);

		data->Index = datas.Count();
		data->Id = id;
		data->FromBlueprint(reader);
		datas.AddOrSet(data->Id, std::move(data));
	}

	template<typename DataType>
	auto CreateTypedData(Blueprint& blueprint, const DataMap<DataType>& map) -> std::unique_ptr<DataType>
	{
		auto type = String();
		auto reader = BlueprintReader(blueprint);
		reader.ReadChild("DataType", type);
		return map.CreateData(type);
	}

	template<typename DataType>
	void LoadDatas(StringView name, Map<String, std::unique_ptr<DataType>>& datas, const DataMap<DataType>& map)
	{
		auto contents = ApplicationDirectory().MovingDown("Assets/Data").MovingDown(name).GetContents();
		for (auto& file : contents.Files)
		{
			auto blueprint = Blueprint();
			if (ReadFile(file, blueprint))
				ReadData(file.GetName(), blueprint, CreateTypedData(blueprint, map), datas);
		}
	}

	template<typename DataType>
	void LoadDatas(StringView name, Map<String, std::unique_ptr<DataType>>& datas)
	{
		auto contents = ApplicationDirectory().MovingDown("Assets/Data").MovingDown(name).GetContents();
		for (auto& file : contents.Files)
		{
			auto blueprint = Blueprint();
			if (ReadFile(file, blueprint))
				ReadData(file.GetName(), blueprint, std::make_unique<DataType>(), datas);
		}
	}
}

void DataManager::Load()
{
	LoadDatas("Equipment", _equipment);
	LoadDatas("Items", _items);
	LoadDatas("Weapons", _weapons);
	LoadDatas("Upgrades", _upgrades, UpgradeData::UpgradeMap);
	LoadDatas("Components", _components);
	LoadDatas("Augments", _augments);
	LoadDatas("Skeletons", _skeletons, SkeletonData::SkeletonMap);
	LoadDatas("Chasses", _chasses);
	LoadDatas("Characters", _characters, CharacterData::CharacterDataMap);
}

auto DataManager::AugmentDatas() const -> SequenceView<std::unique_ptr<AugmentData>>
{
	return _augments.Items();
}

auto DataManager::CharacterDatas() const -> SequenceView<std::unique_ptr<CharacterData>>
{
	return _characters.Items();
}

auto DataManager::ChassisDatas() const -> SequenceView<std::unique_ptr<ChassisData>>
{
	return _chasses.Items();
}

auto DataManager::ComponentDatas() const -> SequenceView<std::unique_ptr<ComponentData>>
{
	return _components.Items();
}

auto DataManager::EquipmentDatas() const -> SequenceView<std::unique_ptr<EquipmentData>>
{
	return _equipment.Items();
}

auto DataManager::ItemDatas() const -> SequenceView<std::unique_ptr<ItemData>>
{
	return _items.Items();
}

auto DataManager::SkeletonDatas() const -> SequenceView<std::unique_ptr<SkeletonData>>
{
	return _skeletons.Items();
}

auto DataManager::UpgradeDatas() const -> SequenceView<std::unique_ptr<UpgradeData>>
{
	return _upgrades.Items();
}

auto DataManager::WeaponDatas() const -> SequenceView<std::unique_ptr<WeaponData>>
{
	return _weapons.Items();
}

auto DataManager::GetAugmentData(int index) const -> AugmentData*
{
	assert(index >= 0 && index < _augments.Count());
	return _augments.ItemAtIndex(index).get();
}

auto DataManager::GetCharacterData(int index) const -> CharacterData*
{
	assert(index >= 0 && index < _chasses.Count());
	return _characters.ItemAtIndex(index).get();
}

auto DataManager::GetChassisData(int index) const -> ChassisData*
{
	assert(index >= 0 && index < _chasses.Count());
	return _chasses.ItemAtIndex(index).get();
}

auto DataManager::GetComponentData(int index) const -> ComponentData*
{
	assert(index >= 0 && index < _components.Count());
	return _components.ItemAtIndex(index).get();
}

auto DataManager::GetEquipmentData(int index) const -> EquipmentData*
{
	assert(index >= 0 && index < _equipment.Count());
	return _equipment.ItemAtIndex(index).get();
}

auto DataManager::GetItemData(int index) const -> ItemData*
{
	assert(index >= 0 && index < _items.Count());
	return _items.ItemAtIndex(index).get();
}

auto DataManager::GetSkeletonData(int index) const -> SkeletonData*
{
	assert(index >= 0 && index < _skeletons.Count());
	return _skeletons.ItemAtIndex(index).get();
}

auto DataManager::GetUpgradeData(int index) const -> UpgradeData*
{
	assert(index >= 0 && index < _upgrades.Count());
	return _upgrades.ItemAtIndex(index).get();
}

auto DataManager::GetWeaponData(int index) const -> WeaponData*
{
	assert(index >= 0 && index < _weapons.Count());
	return _weapons.ItemAtIndex(index).get();
}

auto DataManager::GetAugmentData(StringView id) const -> AugmentData*
{
	return GetAugmentData(_augments.GetIndex(id));
}

auto DataManager::GetCharacterData(StringView id) const -> CharacterData*
{
	return GetCharacterData(_characters.GetIndex(id));
}

auto DataManager::GetChassisData(StringView id) const -> ChassisData*
{
	return GetChassisData(_chasses.GetIndex(id));
}

auto DataManager::GetComponentData(StringView id) const -> ComponentData*
{
	return GetComponentData(_components.GetIndex(id));
}

auto DataManager::GetEquipmentData(StringView id) const -> EquipmentData*
{
	return GetEquipmentData(_equipment.GetIndex(id));
}

auto DataManager::GetItemData(StringView id) const -> ItemData*
{
	return GetItemData(_items.GetIndex(id));
}

auto DataManager::GetSkeletonData(StringView id) const -> SkeletonData*
{
	return GetSkeletonData(_skeletons.GetIndex(id));
}

auto DataManager::GetUpgradeData(StringView id) const -> UpgradeData*
{
	return GetUpgradeData(_upgrades.GetIndex(id));
}

auto DataManager::GetWeaponData(StringView id) const -> WeaponData*
{
	return GetWeaponData(_weapons.GetIndex(id));
}
