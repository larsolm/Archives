#pragma once

#include "Collision/ProjectileCollision.h"

#include <Pargon.h>

using namespace Pargon;

class LinearCollision : public ProjectileCollision
{
public:
	void Update(float elapsed) override;
};
