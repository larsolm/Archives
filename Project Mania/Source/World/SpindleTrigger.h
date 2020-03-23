#pragma once

#include "World/Interaction.h"
#include "World/Storages/PermanentStaticallyTypedStorage.h"
#include "World/Storages/StorageId.h"

class SpindleTrigger : public Interaction
{
public:
	using Interaction::Interaction;
	using Storage = PermanentStaticallyTypedStorage<SpindleTrigger>;
	using IdType = GameObjectId<Storage>;

	static constexpr int MaximumCount = 16;

	IdType Id;
	WorldNodeData* Connection;

	void Initialize();
	void Interact(Player& player) override;
	void FromBlueprint(BlueprintReader& reader);
};
