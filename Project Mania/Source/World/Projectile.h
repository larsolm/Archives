#pragma once

#include "Collision/ProjectileCollision.h"
#include "Game/WeaponAbilities.h"
#include "Game/WeaponAttributes.h"
#include "World/DynamicGameObject.h"
#include "World/Storages/RemovableStaticallyTypedStorage.h"
#include "World/Storages/StorageId.h"

#include <PargonScene.h>

class Character;
class CollisionLine;

class Projectile : public DynamicGameObject
{
public:
	using DynamicGameObject::DynamicGameObject;
	using Storage = RemovableStaticallyTypedStorage<Projectile>;
	using IdType = GameObjectId<Storage>;

	static constexpr int MaximumCount = 1024;

	IdType Id;

	Character* Owner;

	Angle Direction;

	Vector2 Velocity;
	Rotation AngularVelocity;

	float Weight;
	float Damage;

	std::unique_ptr<ProjectileCollision> Collision;

	auto GetBounds() const -> Rectangle override;

	void Initialize(Point2 position, Angle rotation);

	void Start();
	void Stop();
	void Update(float elapsed);
	void Interpolate(float interpolation);
	void Animate(float elapsed);

	void ApplyAttributes(const WeaponAbilities& abilities, const WeaponAttributes& attributes, const WeaponAttributeModifiers& modifiers);
	void Collided(Character& character);
	void Collided(CollisionLine& line);

private:
	Model* _model;

	void OnNodeChanged(const WorldNodeData& previous) override;
};

inline
auto Projectile::GetBounds() const -> Rectangle
{
	return { ProjectedPosition.X, ProjectedPosition.Y, 0.0f, 0.0f };
}
