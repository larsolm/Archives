#pragma once

#include "Character/Processors/GroundProcessor.h"

class CollisionLine;

class EnemyProcessor : public GroundProcessor
{
public:
	void Update(float elapsed) override;
	void ProjectileHit(Projectile& projectile) override;

protected:
	void OnDeath() override;

private:
	float _horizontalMoveVelocity = 0.0f;

	void UpdateEnvironmentVelocity(float elapsed);
	void UpdateMoveVelocity(float elapsed);

	auto GetGroundVelocity(float velocity) -> float;
};
