#include "Pch.h"
#include "Collision/DynamicCollision.h"
#include "Collision/LinearCollision.h"
#include "World/Projectile.h"
#include "Game/Game.h"

#include <PargonAnimation.h>

void Projectile::Initialize(Point2 position, Angle rotation)
{
	ProjectedPosition = position;
	Position = Game()->World.GetRealPosition(position, Node().Data);
	PreviousPosition = Position;
	Direction = rotation;

	_model = Game()->Rendering.CreateDebugModel(Rectangle{ -0.25f, -0.125f, 0.5f, 0.25f }, Node().Data.IsSpindle, 0.0_radians, Node().Data.ZPosition, Blue);
}

void Projectile::Start()
{
}

void Projectile::Stop()
{
}

void Projectile::Update(float elapsed)
{
	PreviousPosition = Position;
	ProjectedPosition += Velocity * elapsed;
	
	Velocity.Y += Weight * Node().Data.GravityStrength * elapsed;

	Collision->Update(elapsed);

	Position = Game()->World.GetRealPosition(ProjectedPosition, Node().Data);
}

void Projectile::Interpolate(float interpolation)
{
	auto position = Pargon::Interpolate(PreviousPosition, Position, interpolation);
	auto yaw = Node().Data.IsSpindle ? 90_degrees : 0.0_radians;
	auto pitch = Node().Data.IsSpindle ? -Direction : 0.0_radians;
	auto roll =  Node().Data.IsSpindle ? Node().Data.WorldAngle : Direction + Node().Data.WorldAngle;

	_model->Transform = Matrix4x4::CreateTransform({ position.X, position.Y, position.Z }, { 1.0f, 1.0f, 1.0f }, Quaternion::CreateFromEulerAngles(yaw, pitch, roll), {});
}

void Projectile::Animate(float elapsed)
{
}

void Projectile::ApplyAttributes(const WeaponAbilities& abilities, const WeaponAttributes& attributes, const WeaponAttributeModifiers& modifiers)
{
	auto speed = attributes.GetAttribute(modifiers, WeaponAttribute::Speed);
	auto weight = attributes.GetAttribute(modifiers, WeaponAttribute::Weight);

	Weight = abilities.HasAbility(WeaponAbility::Ballistic) ? weight : 0.0f;
	Damage = attributes.GetAttribute(modifiers, WeaponAttribute::Damage);

	if (abilities.HasAbility(WeaponAbility::Dynamic))
		Collision = std::make_unique<DynamicCollision>();
	else 
		Collision = std::make_unique<LinearCollision>();

	Collision->Projectile = this;

	Velocity = Vector2::CreateFromDirection(Direction) * speed;
}

void Projectile::Collided(Character& character)
{
	character.Processor->ProjectileHit(*this);

	Game()->World.DestroyProjectile(*this);
}

void Projectile::Collided(CollisionLine& line)
{
	Game()->World.DestroyProjectile(*this);
}

void Projectile::OnNodeChanged(const WorldNodeData& previous)
{
	auto angleChange = previous.WorldAngle - Node().Data.WorldAngle;

	Direction += angleChange;
	Velocity.Rotate(angleChange);
}
