#include "Pch.h"
#include "Rendering/PlayerCamera.h"
#include "Character/Characters/Player.h"
#include "World/WorldNode.h"
#include "Game/Game.h"

#include <PargonAnimation.h>

void PlayerCamera::Initialize()
{
	Reposition({ 0.0f, 0.0f, -20.0f });
	Reorient(Quaternion::CreateIdentity());
}

void PlayerCamera::Interpolate(float interpolation)
{
	auto position = Pargon::Interpolate(Player->PreviousPosition, Player->Position, interpolation);
	auto rotation = Player->Node().Data.GetCameraAngle();
	auto offset = Vector3{ 0.0f, 0.0f, -20.0f }.Rotated(rotation);

	_targetPosition = position + offset;
	_targetRotation = rotation;
}

void PlayerCamera::Animate(Time elasped)
{
	auto position = GetPosition();
	auto rotation = GetRotation();

	auto time = static_cast<float>(elasped.InSeconds());
	auto moveAmount = time * MoveSpeed;
	auto rotateAmount = time * RotationSpeed;

	auto direction = _targetPosition - position;
	auto move = direction.GetLength();
	if (move < moveAmount)
		position = _targetPosition;
	else
		position += direction * (moveAmount / move);

	auto theta = 2 * ArcCosine(_targetRotation.X * rotation.X + _targetRotation.Y * rotation.Y + _targetRotation.Z * rotation.Z + _targetRotation.W * rotation.W).InDegrees();
	auto distance = IsGreaterThan(theta, 180.0f) ? 360.0f - theta : theta;
	if (distance < rotateAmount)
		rotation = _targetRotation;
	else
		rotation.Interpolate(_targetRotation, rotateAmount / distance);

	Reposition(position);
	Reorient(rotation);

	Update(elasped);
}
