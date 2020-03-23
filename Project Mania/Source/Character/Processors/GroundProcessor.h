#pragma once

#include "Game/Duration.h"
#include "Character/Processor.h"

#include <Pargon.h>
#include <PargonShapes.h>

using namespace Pargon;

class CollisionLine;

class GroundProcessor : public Processor
{
public:
	auto OnGround() const -> bool;
	auto LeftGround() const -> CollisionLine*;
	auto RightGround() const -> CollisionLine*;

protected:
	CollisionLine* _currentGround = nullptr;
	CollisionLine* _leftGround = nullptr;
	CollisionLine* _rightGround = nullptr;

	void UpdateGround(float elapsed);

	auto GetGroundCheckBounds() const -> Shapes::Rectangle;
	auto GetSlope() -> float;
};

inline
auto GroundProcessor::LeftGround() const -> CollisionLine*
{
	return _leftGround;
}

inline
auto GroundProcessor::RightGround() const -> CollisionLine*
{
	return _rightGround;
}
