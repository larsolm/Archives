#pragma once

#include "Character/Controller.h"

#include <Pargon.h>

using namespace Pargon;

class EnemyController : public Controller
{
public:
	void Update(float elapsed) override;
};
