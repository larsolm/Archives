#pragma once

#include <Pargon.h>
#include <PargonShapes.h>

using namespace Pargon;
using namespace Pargon::Shapes;

class Character;
class CollisionLine;

class CharacterCollision
{
public:
	Character* Character;

	List<CollisionLine*> Ceilings;
	List<CollisionLine*> Grounds;
	List<CollisionLine*> LeftWalls;
	List<CollisionLine*> RightWalls;

	Angle MaxSlopeAngle = 50_degrees;
	Angle MaxCeilingAngle = 50_degrees;

	void Reset();
	void Initialize();
	void Update(float elapsed);

	auto Size() const -> Vector2;
	auto CollisionPolygon() const -> SequenceView<Vector2>;
	void SetCollisionPolygon(SequenceView<Vector2> polygon);
	auto GetCollisionBounds(Point2 position, Vector2 velocity) const -> Rectangle;

private:
	Vector2 _size;
	List<Vector2> _rigidPolygon;
	List<Vector2> _normals;

	auto Iterate(SequenceView<CollisionLine*> ceilings, SequenceView<CollisionLine*> grounds, SequenceView<CollisionLine*> leftWalls, SequenceView<CollisionLine*> rightWalls, Vector2& velocity, float& elapsed) -> bool;
};

inline
auto CharacterCollision::Size() const -> Vector2
{
	return _size;
}

inline
auto CharacterCollision::CollisionPolygon() const -> SequenceView<Vector2>
{
	return _rigidPolygon;
}
