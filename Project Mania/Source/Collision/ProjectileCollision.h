#pragma once

#include <Pargon.h>

using namespace Pargon;

class Projectile;

class ProjectileCollision
{
public:
	Projectile* Projectile;

	virtual void Update(float elapsed) = 0;
};
