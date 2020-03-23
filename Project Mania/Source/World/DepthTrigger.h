#pragma once

#include "World/Trigger.h"
#include "World/Storages/PermanentStaticallyTypedStorage.h"
#include "World/Storages/StorageId.h"

class DepthTrigger : public Trigger
{
public:
	using Trigger::Trigger;
	using Storage = PermanentStaticallyTypedStorage<DepthTrigger>;
	using IdType = GameObjectId<Storage>;

	static constexpr int MaximumCount = 16;

	IdType Id;

	bool CanMoveForward = false;
	bool CanMoveMiddle = false;
	bool CanMoveBackward = false;
	bool ForceMiddle = false;

	auto GetBounds() const -> Rectangle override;

	void Initialize() override;
	void Execute(Character& player) override;

	void FromBlueprint(BlueprintReader& reader);

private:
	Rectangle _bounds;
};

inline
auto DepthTrigger::GetBounds() const -> Rectangle
{
	return _bounds;
}
