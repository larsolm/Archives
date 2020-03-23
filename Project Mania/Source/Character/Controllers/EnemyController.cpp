#include "Pch.h"
#include "Character/Controllers/EnemyController.h"
#include "Game/Game.h"

using namespace Pargon;

void EnemyController::Update(float elapsed)
{
	Horizontal = 0.0f;
	Aim = { -1.0f, 0.0f };
	WeaponPrimary = true;
}
