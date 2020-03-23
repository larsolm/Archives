#pragma once

#include "Game/Duration.h"
#include "Character/Processors/GroundProcessor.h"

#include <Pargon.h>
#include <PargonShapes.h>

using namespace Pargon;

class Player;
class CollisionLine;

class PlayerProcessor : public GroundProcessor
{
public:
	Player* Player;

	void Initialize() override;
	void Update(float elapsed) override;
	void ProjectileHit(Projectile& projectile) override;

protected:
	void OnShieldDepleted() override;
	void OnDeath() override;

private:
	static constexpr Duration _hopFrames = 3_ticks;
	static constexpr float _healthRechargeTime = 2.0f;
	static constexpr float _shieldRechargeTimeout = 5.0;

	Duration _jumpTime = Duration::Infinite();

	float _horizontalMoveVelocity = 0.0f;
	float _shieldRechargeTimer = 0.0;

	bool _canRechargeHealth = true;
	bool _interactReleased = true;

	void UpdateEnvironmentVelocity(float elapsed);
	void UpdateMoveVelocity(float elapsed);
	void UpdateInteract(float elapsed);
	void UpdateRecharge(float elapsed);
	void UpdateParts(float elapsed);
	void UpdateEnemyCollision(float elapsed);

	auto GetJump(bool onGround) -> float;
	auto GetGroundVelocity(float velocity) -> float;
	auto GetAirVelocity(float velocity, float elapsed) -> float;
};
