#pragma once

#include "World/StaticGameObject.h"
#include "World/Storages/PermanentStaticallyTypedStorage.h"
#include "World/Storages/StorageId.h"

enum class CollisionDepth
{
	Back,
	Middle,
	Front
};

template<> auto EnumNames<CollisionDepth> = SetEnumNames
(
	"Back",
	"Middle",
	"Front"
);

class CollisionLine : public StaticGameObject
{
public:
	using Storage = PermanentStaticallyTypedStorage<CollisionLine>;
	using IdType = GameObjectId<Storage>;

	static constexpr int MaximumCount = 1024;

	using StaticGameObject::StaticGameObject;

	IdType Id;
	CollisionDepth Depth = CollisionDepth::Middle;
	Segment2 Line;
	Vector2 Normal;

	float Slope;

	void Initialize();
	void CalculateNormal();
	void FlipNormal();

	void FromBlueprint(BlueprintReader& reader);
};
