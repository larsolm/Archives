#pragma once

#include "Collision/ProjectileCollision.h"

#include <Pargon.h>

using namespace Pargon;

class DynamicCollision : public ProjectileCollision
{
public:
	void Update(float elapsed) override;
};
