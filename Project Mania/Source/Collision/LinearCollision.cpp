#include "Pch.h"
#include "Character/Character.h"
#include "Collision/LinearCollision.h"
#include "Game/Game.h"
#include "World/CollisionLine.h"
#include "World/Projectile.h"

#include <PargonShapes.h>

using namespace Pargon::Shapes;

namespace
{
	auto CanCollide(CollisionLine& line, Vector2 velocity) -> bool
	{
		auto dot = line.Normal.GetDotProduct(velocity);
		return IsLessThan(dot, 0.0f);
	}

	auto CheckCollision(CollisionLine& line, Point2& position, Vector2& velocity, float elapsed) -> bool
	{
		if (CanCollide(line, velocity))
		{
			auto intersection = GetIntersection(line.Line, Segment2{ position, position + (velocity * elapsed) });
			if (intersection.Type != Intersection2Type::None)
			{
				position = intersection.Point1;
				velocity = { 0.0f, 0.0f };
				return true;
			}
		}

		return false;
	}

	auto CheckCollision(Character& character, Point2& position, Vector2& velocity, float elapsed) -> bool
	{
		auto end = position + (velocity * elapsed);
		auto bounds = character.Collision.GetCollisionBounds(character.ProjectedPosition, (character.MoveVelocity + character.EnvironmentVelocity) * elapsed);
		auto intersection = GetIntersection(Segment2{ position, end }, bounds);
		if (intersection.Type != Intersection2Type::None)
		{
			position = intersection.Point1;
			velocity = { 0.0f, 0.0f };
			return true;
		}

		return false;
	}
}

void LinearCollision::Update(float elapsed)
{
	auto lines = Projectile->Node().CollisionLines.All();
	for (auto& line : lines)
	{
		if (CheckCollision(*line, Projectile->ProjectedPosition, Projectile->Velocity, elapsed))
		{
			Projectile->Collided(*line);
			break;
		}
	}

	auto characters = Game()->World.Characters.All();
	for (auto& character : characters)
	{
		if (Projectile->Owner == character.get())
			continue;

		if (CheckCollision(*character, Projectile->ProjectedPosition, Projectile->Velocity, elapsed))
		{
			Projectile->Collided(*character);
			break;
		}
	}
}
