#pragma once

#include <Pargon.h>
#include <PargonScene.h>

using namespace Pargon;

class Player;

class PlayerCamera : public FreeCamera
{
public:
	Player* Player;

	float MoveSpeed = 25.0f;
	float RotationSpeed = 90.0f;

	void Initialize();
	void Interpolate(float intepolation);
	void Animate(Time elapsed);

private:
	Point3 _targetPosition;
	Quaternion _targetRotation;
	Quaternion _previousRotation;
};
