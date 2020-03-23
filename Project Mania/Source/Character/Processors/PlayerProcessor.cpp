#include "Pch.h"
#include "Character/Character.h"
#include "Character/Characters/Player.h"
#include "Character/Processors/PlayerProcessor.h"
#include "Game/Game.h"

using namespace Pargon;
using namespace Pargon::Shapes;

namespace
{
	auto ApplyImpulse(Vector2& velocity, Vector2& amount, Vector2 maximum)
	{
		if (amount.X < 0.0f)
		{
			auto fraction = (maximum.X + velocity.X) / (2 * maximum.X);
			velocity.X += amount.X * Maximum(0.0f, fraction);
		}
		else if (amount.X > 0.0f)
		{
			auto fraction = (maximum.X - velocity.X) / (2 * maximum.X);
			velocity.X += amount.X * Maximum(0.0f, fraction);
		}

		if (amount.Y < 0.0f)
		{
			auto fraction = (maximum.Y + velocity.Y) / (2 * maximum.Y);
			velocity.Y += amount.Y * Maximum(0.0f, fraction);
		}
		else if (amount.Y > 0.0f)
		{
			auto fraction = (maximum.Y - velocity.Y) / (2 * maximum.Y);
			velocity.Y += amount.Y * Maximum(0.0f, fraction);
		}

		amount = { 0.0f, 0.0f };
	}
}

void PlayerProcessor::Initialize()
{
	Player = static_cast<class Player*>(Character);
}

void PlayerProcessor::Update(float elapsed)
{
	UpdateEnemyCollision(elapsed);
	UpdateEnvironmentVelocity(elapsed);
	UpdateMoveVelocity(elapsed);
	UpdateDepth(elapsed);

	Player->Collision.Update(elapsed);

	UpdateInteract(elapsed);
	UpdateParts(elapsed);
	UpdateRecharge(elapsed);
	UpdateGround(elapsed);
}

void PlayerProcessor::ProjectileHit(Projectile& projectile)
{
	_shieldRechargeTimer = 0.0f;

	ApplyDamage(projectile.Damage);
}

void PlayerProcessor::OnShieldDepleted()
{
}

void PlayerProcessor::OnDeath()
{
}

void PlayerProcessor::UpdateEnvironmentVelocity(float elapsed)
{
	auto jump = GetJump(OnGround());
	auto gravity = _currentGround == nullptr ? Player->Node().Data.GravityStrength * Player->Data->Weight * elapsed : 0.0f;

	if (_jumpTime.Ticks() == 0)
		Player->EnvironmentVelocity.Y = jump;
	else
		Player->EnvironmentVelocity.Y += jump;

	if (Player->EnvironmentVelocity.Y > -Player->Data->FallSpeed + gravity)
		Player->EnvironmentVelocity.Y += gravity;
	else if (Player->EnvironmentVelocity.Y > -Player->Data->FallSpeed)
		Player->EnvironmentVelocity.Y = -Player->Data->FallSpeed;
	else if (Player->EnvironmentVelocity.Y < -Player->Data->FallSpeed - gravity)
		Player->EnvironmentVelocity.Y -= gravity;
	else if (Player->EnvironmentVelocity.Y < -Player->Data->FallSpeed)
		Player->EnvironmentVelocity.Y = -Player->Data->FallSpeed;

	Player->EnvironmentVelocity += Player->UncappedImpulse;

	Player->WindImpulse *= Player->Data->Aerodynamics;
	Player->WindVelocity *= Player->Data->Aerodynamics;

	ApplyImpulse(Player->EnvironmentVelocity, Player->CappedImpulse, { Maximum(Player->Data->ForwardSpeed, Player->Data->BackwardSpeed), Player->Data->FallSpeed });
	ApplyImpulse(Player->EnvironmentVelocity, Player->WindImpulse, Player->WindVelocity);

	Player->WindImpulse = { 0.0f, 0.0f };
	Player->CappedImpulse = { 0.0f, 0.0f };
	Player->UncappedImpulse = { 0.0f, 0.0f };
}

void PlayerProcessor::UpdateMoveVelocity(float elapsed)
{
	if (!Player->Collision.LeftWalls.IsEmpty() || !Player->Collision.RightWalls.IsEmpty())
		_horizontalMoveVelocity = 0.0f;

	_horizontalMoveVelocity = OnGround() ? GetGroundVelocity(_horizontalMoveVelocity) : GetAirVelocity(_horizontalMoveVelocity, elapsed);

	auto groundAngle = ArcTangent(1.0f, GetSlope());
	auto direction = Vector2::CreateFromDirection(Angle(groundAngle));
	Player->MoveVelocity = direction * _horizontalMoveVelocity;
}

void PlayerProcessor::UpdateInteract(float elapsed)
{
	// THIS WILL EVENTUALLY BE AN ALL INTERACTIONS THING.
	auto interactions = Player->Node().SpindleTriggers.All();
	auto smallestInteraction = std::numeric_limits<float>::max();
	auto playerBounds = Player->GetBounds();

	Player->CurrentInteraction = nullptr;

	_interactReleased = _interactReleased || !Player->Controller->Interact;

	for (auto& interaction : interactions)
	{
		auto interactionBounds = interaction->GetBounds();
		if (playerBounds.Left() < interactionBounds.Right() && playerBounds.Right() > interactionBounds.Left() && playerBounds.Top() > interactionBounds.Bottom() && playerBounds.Bottom() < interactionBounds.Top())
		{
			auto center = interactionBounds.MiddleCenter();
			auto distance = (center - Player->ProjectedPosition).GetLengthSquared();
			if (distance < smallestInteraction)
			{
				smallestInteraction = distance;
				Player->CurrentInteraction = interaction.get();
			}
		}
	}

	if (Player->CurrentInteraction != nullptr && Player->Controller->Interact && _interactReleased)
	{
		_interactReleased = false;

		Player->CurrentInteraction->Interact(*Player);
	}
}

void PlayerProcessor::UpdateParts(float elapsed)
{
	if (Player->Controller->WeaponSwitch)
	{
		if (!Player->Chassis.Weapons.IsEmpty())
		{
			Player->ActiveWeaponSlot++;
			if (Player->ActiveWeaponSlot >= Player->Chassis.Weapons.Count())
				Player->ActiveWeaponSlot = 0;
		}
	}
	
	if (Player->Controller->ItemOne)
	{
		if (Player->Chassis.Items.Count() >= 1)
			Player->ActiveItemSlot = 0;
	}
	else if (Player->Controller->ItemTwo)
	{
		if (Player->Chassis.Items.Count() >= 2)
			Player->ActiveItemSlot = 1;
	}
	else if (Player->Controller->ItemThree)
	{
		if (Player->Chassis.Items.Count() >= 3)
			Player->ActiveItemSlot = 2;
	}
	else if (Player->Controller->ItemFour)
	{
		if (Player->Chassis.Items.Count() >= 4)
			Player->ActiveItemSlot = 3;
	}
}

void PlayerProcessor::UpdateRecharge(float elapsed)
{
	if (_canRechargeHealth)
	{
		auto maxHealth = Player->Chassis.GetMaxHealth();
		auto regen = maxHealth / _healthRechargeTime;
		Player->Health += regen * elapsed;

		if (Player->Health > maxHealth)
			Player->Health = maxHealth;
	}

	_shieldRechargeTimer += elapsed;
	if (_shieldRechargeTimer > _shieldRechargeTimeout)
	{
		auto maxShield = Player->Chassis.GetMaxShield();
		auto regenTime = Player->Chassis.GetShieldRegen();
		auto regen = maxShield / regenTime;

		Player->Shield += regen * elapsed;

		if (Player->Shield > maxShield)
			Player->Shield = maxShield;
	}
}

void PlayerProcessor::UpdateEnemyCollision(float elapsed)
{
	auto playerVelocity = (Player->MoveVelocity + Player->EnvironmentVelocity) * elapsed;
	auto playerBounds = Player->Collision.GetCollisionBounds(Player->ProjectedPosition, playerVelocity);
	auto characters = Game()->World.Characters.All();
	for (auto& character : characters)
	{
		if (character.get() == Player)
			continue;

		auto velocity = (character->MoveVelocity + character->EnvironmentVelocity) * elapsed;
		auto bounds = character->Collision.GetCollisionBounds(character->ProjectedPosition, velocity);
		if (playerBounds.Left() <= bounds.Right() && playerBounds.Right() > bounds.Left() && playerBounds.Top() > bounds.Bottom() && playerBounds.Bottom() < bounds.Top())
		{
			auto difference = Player->ProjectedPosition - character->ProjectedPosition;
			Player->UncappedImpulse += (difference.Normalized() * 5.0f);
			ApplyDamage(character->Data->ContactDamage);
		}
	}
}

auto PlayerProcessor::GetJump(bool onGround) -> float
{
	auto hangFrames = Duration::FromSeconds(Player->Data->HangTime, Game()->TickRate());

	if (Player->Collision.Ceilings.IsEmpty())
	{
		if (((Player->Controller->Jump && _jumpTime.IsInfinite() && onGround) || (!_jumpTime.IsInfinite() && _jumpTime < _hopFrames)))
		{
			_jumpTime++;
			return Player->Data->Hops;
		}

		if (Player->Controller->Jump && _jumpTime >= _hopFrames && _jumpTime < hangFrames)
		{
			_jumpTime++;
			return Player->Data->HangAmount;
		}
	}

	_jumpTime = onGround ? Duration::Infinite() : hangFrames;

	return 0.0f;
}

auto PlayerProcessor::GetGroundVelocity(float velocity) -> float
{
	auto flipped = Player->Controller->Aim.X < 0.0f;
	auto leftSpeed = flipped ? Player->Data->ForwardSpeed : Player->Data->BackwardSpeed;
	auto rightSpeed = flipped ? Player->Data->BackwardSpeed : Player->Data->ForwardSpeed;
	auto requestedVelocity = Player->Controller->Horizontal * (Player->Controller->Horizontal < 0.0f ? leftSpeed : rightSpeed);

	if (velocity > requestedVelocity + Player->Data->Agility)
		velocity -= Player->Data->Agility;
	else if (velocity < requestedVelocity - Player->Data->Agility)
		velocity += Player->Data->Agility;
	else
		velocity = requestedVelocity;

	return velocity;
}

auto PlayerProcessor::GetAirVelocity(float velocity, float elapsed) -> float
{
	auto flipped = Player->Controller->Aim.X < 0.0f;
	auto leftSpeed = flipped ? Player->Data->ForwardSpeed : Player->Data->BackwardSpeed;
	auto rightSpeed = flipped ? Player->Data->BackwardSpeed : Player->Data->ForwardSpeed;
	auto acceleration = Player->Data->Maneuverability * Player->Controller->Horizontal * elapsed;

	if (acceleration < 0.0f)
	{
		if (velocity + acceleration >= -leftSpeed)
			velocity += acceleration;
		else if (velocity > -leftSpeed)
			velocity = -leftSpeed;
	}
	else if (acceleration > 0.0f)
	{
		if (velocity + acceleration <= rightSpeed)
			velocity += acceleration;
		else if (velocity < rightSpeed)
			velocity = rightSpeed;
	}

	return velocity;
}
