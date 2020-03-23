#pragma once

#include "World/StaticGameObject.h"

class Character;

class Trigger : public StaticGameObject
{
public:
	using StaticGameObject::StaticGameObject;

	Polygon2 Polygon;

	virtual void Initialize();
	virtual void Execute(Character& character) = 0;
};
