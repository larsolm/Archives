#pragma once

#include "World/StaticGameObject.h"

class Player;

class Interaction : public StaticGameObject
{
public:
	using StaticGameObject::StaticGameObject;

	virtual void Interact(Player& player) = 0;
};
