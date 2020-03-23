#pragma once

#include "Character/Chassis.h"
#include "Character/Controller.h"
#include "Character/Processor.h"
#include "Collision/CharacterCollision.h"
#include "World/DynamicGameObject.h"
#include "World/CollisionLine.h"
#include "World/Storages/RemovableDynamicallyTypedStorage.h"
#include "World/Storages/StorageId.h"

struct CharacterData;

class Character : public DynamicGameObject
{
public:
	using DynamicGameObject::DynamicGameObject;
	using Storage = RemovableDynamicallyTypedStorage<Character, CharacterData>;
	using IdType = GameObjectId<Storage>;

	static constexpr int MaximumCount = 128;

	IdType Id;

	CharacterData* Data;

	Chassis Chassis;
	CharacterCollision Collision;

	std::unique_ptr<Controller> Controller;
	std::unique_ptr<Processor> Processor;

	CollisionDepth Depth = CollisionDepth::Middle;
	bool CanMoveToFront = false;
	bool CanMoveToMiddle = false;
	bool CanMoveToBack = false;
	bool ForceMiddle = false;

	Vector2 MoveVelocity = { 0.0f, 0.0f };
	Vector2 EnvironmentVelocity = { 0.0f, 0.0f };

	Vector2 CappedImpulse = { 0.0f, 0.0f };
	Vector2 UncappedImpulse = { 0.0f, 0.0f };
	Vector2 WindImpulse = { 0.0f, 0.0f };
	Vector2 WindVelocity = { 0.0f, 0.0f };

	int ActiveWeaponSlot = -1;
	int ActiveItemSlot = -1;

	float Health;
	float Shield;

	auto GetBounds() const -> Rectangle override;

	void Initialize(Point2 position);
	
	virtual void Start() {};
	virtual void Stop() {};
	virtual void Update(float elapsed);
	virtual void Interpolate(float interpolation);
	virtual void Animate(float elapsed);

protected:
	void OnNodeChanged(const WorldNodeData& previous) override;
};

inline
auto Character::GetBounds() const -> Rectangle
{
	return Collision.GetCollisionBounds(ProjectedPosition, {});
}
