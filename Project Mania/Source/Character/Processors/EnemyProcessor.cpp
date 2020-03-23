#include "Pch.h"
#include "Character/Character.h"
#include "Character/Processors/EnemyProcessor.h"
#include "Data/CharacterData.h"
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

void EnemyProcessor::Update(float elapsed)
{	
	UpdateEnvironmentVelocity(elapsed);
	UpdateMoveVelocity(elapsed);

	Character->Collision.Update(elapsed);

	UpdateGround(elapsed);
}

void EnemyProcessor::ProjectileHit(Projectile& projectile)
{
	ApplyDamage(projectile.Damage);
}

void EnemyProcessor::OnDeath()
{
	Game()->World.DestroyCharacter(*Character);
}

void EnemyProcessor::UpdateEnvironmentVelocity(float elapsed)
{
	auto gravity = _currentGround == nullptr ? Character->Data->Weight * Character->Node().Data.GravityStrength * elapsed : 0.0f;

	if (Character->EnvironmentVelocity.Y > -Character->Data->FallSpeed + gravity)
		Character->EnvironmentVelocity.Y += gravity;
	else if (Character->EnvironmentVelocity.Y > -Character->Data->FallSpeed)
		Character->EnvironmentVelocity.Y = -Character->Data->FallSpeed;
	else if (Character->EnvironmentVelocity.Y < -Character->Data->FallSpeed - gravity)
		Character->EnvironmentVelocity.Y -= gravity;
	else if (Character->EnvironmentVelocity.Y < -Character->Data->FallSpeed)
		Character->EnvironmentVelocity.Y = -Character->Data->FallSpeed;

	Character->EnvironmentVelocity += Character->UncappedImpulse;

	Character->WindImpulse *= Character->Data->Aerodynamics;
	Character->WindVelocity *= Character->Data->Aerodynamics;

	ApplyImpulse(Character->EnvironmentVelocity, Character->CappedImpulse, { Maximum(Character->Data->ForwardSpeed, Character->Data->BackwardSpeed), Character->Data->FallSpeed });
	ApplyImpulse(Character->EnvironmentVelocity, Character->WindImpulse, Character->WindVelocity);

	Character->WindImpulse = { 0.0f, 0.0f };
	Character->CappedImpulse = { 0.0f, 0.0f };
	Character->UncappedImpulse = { 0.0f, 0.0f };
}

void EnemyProcessor::UpdateMoveVelocity(float elapsed)
{
	if (!Character->Collision.LeftWalls.IsEmpty() || !Character->Collision.RightWalls.IsEmpty())
		_horizontalMoveVelocity = 0.0f;

	_horizontalMoveVelocity = OnGround() ? GetGroundVelocity(_horizontalMoveVelocity) : 0.0f;

	auto groundAngle = ArcSine(GetSlope());
	auto direction = Vector2::CreateFromDirection(Angle(groundAngle));
	Character->MoveVelocity = direction * _horizontalMoveVelocity;
}

auto EnemyProcessor::GetGroundVelocity(float velocity) -> float
{
	auto flipped = Character->Controller->Aim.X < 0.0f;
	auto leftSpeed = flipped ? Character->Data->ForwardSpeed : Character->Data->BackwardSpeed;
	auto rightSpeed = flipped ? Character->Data->BackwardSpeed : Character->Data->ForwardSpeed;
	auto requestedVelocity = Character->Controller->Horizontal * (Character->Controller->Horizontal < 0.0f ? leftSpeed : rightSpeed);

	if (velocity > requestedVelocity + Character->Data->Agility)
		velocity -= Character->Data->Agility;
	else if (velocity < requestedVelocity - Character->Data->Agility)
		velocity += Character->Data->Agility;
	else
		velocity = requestedVelocity;

	return velocity;
}
