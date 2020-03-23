#include "Pch.h"
#include "Character/Character.h"
#include "Collision/CharacterCollision.h"
#include "Data/CharacterData.h"
#include "Game/Game.h"
#include "World/CollisionLine.h"

namespace
{
	enum class CollisionType
	{
		Ceiling,
		Ground,
		LeftWall,
		RightWall,
		None
	};

	struct Range
	{
		float Min = std::numeric_limits<float>::max();
		float Max = std::numeric_limits<float>::lowest();

		int MinOne = -1;
		int MinTwo = -1;
		int MaxOne = -1;
		int MaxTwo = -1;
	};

	struct ProjectionInfo
	{
		float Time = 0.0f;
		float TimeLast = 1.0f;
		int Face = -1;
	};

	struct CollisionIterationInfo
	{
		float Time = 1.0f;
		Vector2 Normal;
		CollisionType Type = CollisionType::None;
		CollisionLine* Line = nullptr;
	};

	auto ProjectOntoLine(Vector2 axisVector, CollisionLine* line) -> Range
	{
		Range range;

		auto startProjection = axisVector.GetDotProduct({ line->Line.Start.X, line->Line.Start.Y });
		auto endProjection = axisVector.GetDotProduct({ line->Line.End.X, line->Line.End.Y });

		range.Min = Minimum(startProjection, endProjection);
		range.Max = Maximum(startProjection, endProjection);

		return range;
	}

	auto ProjectOntoPolygon(Vector2 axis, SequenceView<Point2> polygon) -> Range
	{
		Range range;

		for (auto i = 0; i < polygon.Count(); i++)
		{
			auto point = polygon.Item(i);
			auto projection = axis.GetDotProduct({ point.X, point.Y });

			if (IsLessThan(projection, range.Min))
			{
				range.MinOne = i;
				range.MinTwo = -1;
				range.Min = projection;
			}
			else if (IsCloseTo(projection, range.Min))
			{
				range.MinTwo = i;
			}

			if (IsGreaterThan(projection, range.Max))
			{
				range.MaxOne = i;
				range.MaxTwo = -1;
				range.Max = projection;
			}
			else if (IsCloseTo(projection, range.Max))
			{
				range.MaxTwo = i;
			}
		}

		return range;
	}

	void UpdatePolygon(List<Point2>& polygon, SequenceView<Vector2> rigidPolygon, Point2 position)
	{
		polygon.Clear();
		for (auto vertex : rigidPolygon)
			polygon.Add(position + vertex);
	}

	auto CheckLastTime(ProjectionInfo& info, float time) -> bool
	{
		info.TimeLast = Minimum(time, info.TimeLast);
		return info.Time <= info.TimeLast;
	}

	auto CheckFirstTime(ProjectionInfo& info, float time, const Range& player, const Range& line, int lastVertex) -> bool
	{
		if (time >= info.Time)
		{
			if (time > 1.0f || time <= 0.0f)
				return false;

			info.Time = time;
			info.Face = -1;

			if (!IsCloseTo(line.Min, line.Max))
			{
				if (player.MinTwo != -1)
					info.Face = player.MinTwo == lastVertex ? player.MinTwo : player.MinOne;
				else if (player.MaxTwo != -1)
					info.Face = player.MaxTwo == lastVertex ? player.MaxTwo : player.MaxOne;
			}
		}

		return true;
	}

	auto TestAxis(ProjectionInfo& info, CollisionLine* line, SequenceView<Point2> polygon, Vector2 axis, Vector2 velocity)
	{
		auto last = polygon.LastIndex();
		auto speed = axis.GetDotProduct(velocity);
		auto lineProjection = ProjectOntoLine(axis, line);
		auto playerProjection = ProjectOntoPolygon(axis, polygon);
		auto minToMaxTime = (lineProjection.Min - playerProjection.Max) / speed;
		auto maxToMinTime = (lineProjection.Max - playerProjection.Min) / speed;

		// Super special case for overlaping axis when perpendicular and overlapping on a single edge
		if ((!IsCloseTo(lineProjection.Min, lineProjection.Max) && (IsCloseTo(lineProjection.Min, playerProjection.Max) || IsCloseTo(lineProjection.Max, playerProjection.Min))) && IsCloseTo(axis.GetDotProduct(line->Normal), 0.0f))
			return false;

		if (IsLessThan(playerProjection.Max, lineProjection.Min)) // player initially on ‘left’ of line
		{
			if (speed <= 0.0f)
				return false;

			if (!CheckFirstTime(info, minToMaxTime, playerProjection, lineProjection, last))
				return false;

			if (!CheckLastTime(info, maxToMinTime))
				return false;
		}
		else if (IsLessThan(lineProjection.Max, playerProjection.Min)) // player initially on ‘right’ of line
		{
			if (speed >= 0.0f)
				return false;

			if (!CheckFirstTime(info, maxToMinTime, playerProjection, lineProjection, last))
				return false;

			if (!CheckLastTime(info, minToMaxTime))
				return false;
		}
		else // player and line initially overlap
		{
			if (speed > 0.0f)
			{
				if (!CheckLastTime(info, maxToMinTime))
					return false;
			}
			else if (speed < 0.0f)
			{
				if (!CheckLastTime(info, minToMaxTime))
					return false;
			}
		}

		return true;
	}

	void CheckCollision(CollisionIterationInfo& result, CollisionLine* line, SequenceView<Point2> polygon, SequenceView<Vector2> axes, Vector2 velocity, CollisionType type)
	{
		ProjectionInfo info;

		if (!TestAxis(info, line, polygon, line->Normal, velocity))
			return;

		for (auto axis : axes)
		{
			if (!TestAxis(info, line, polygon, axis, velocity))
				return;
		}

		if (info.Time < result.Time)
		{
			result.Line = line;
			result.Type = type;
			result.Time = info.Time;
			result.Normal = info.Face == -1 ? line->Normal : -axes.Item(info.Face);
		}
	}

	auto CanCollide(CollisionLine* line, Vector2 velocity, CollisionDepth depth, bool forceMiddle) -> bool
	{
		auto dot = line->Normal.GetDotProduct(velocity);
		return IsLessThan(dot, 0.0f) && (line->Depth == depth || (line->Depth == CollisionDepth::Middle && forceMiddle));
	}

	void AOBBCollision(CollisionIterationInfo& result, SequenceView<CollisionLine*> lines, SequenceView<Point2> polygon, SequenceView<Vector2> axes, Vector2 velocity, CollisionType type, CollisionDepth depth, bool forceMiddle)
	{
		for (auto line : lines)
		{
			if (CanCollide(line, velocity, depth, forceMiddle))
				CheckCollision(result, line, polygon, axes, velocity, type);
		}
	}

	auto GetCollisionInfo(SequenceView<CollisionLine*> ceilings, SequenceView<CollisionLine*> grounds, SequenceView<CollisionLine*> leftWalls, SequenceView<CollisionLine*> rightWalls, SequenceView<Vector2> polygon, SequenceView<Vector2> axes, Point2 position, Vector2 velocity, CollisionDepth depth, bool forceMiddle, float elapsed) -> CollisionIterationInfo
	{
		static List<Point2> _polygon;

		CollisionIterationInfo result;

		UpdatePolygon(_polygon, polygon, position);

		AOBBCollision(result, grounds, _polygon, axes, velocity, CollisionType::Ground, depth, forceMiddle);
		AOBBCollision(result, ceilings, _polygon, axes, velocity, CollisionType::Ceiling, depth, forceMiddle);
		AOBBCollision(result, leftWalls, _polygon, axes, velocity, CollisionType::LeftWall, depth, forceMiddle);
		AOBBCollision(result, rightWalls, _polygon, axes, velocity, CollisionType::RightWall, depth, forceMiddle);

		return result;
	}

	auto ResolveVelocity(Vector2 velocity, Vector2 normal) -> Vector2
	{
		return velocity - (normal * velocity.GetDotProduct(normal));
	}

	void ResolvePenetration(List<CollisionLine*>& lines, CollisionIterationInfo& info, Point2& position, Vector2& velocity, Vector2& move, Vector2& environment, float& elapsed)
	{
		position += velocity * info.Time * elapsed;
		elapsed *= 1 - info.Time;
		lines.Add(info.Line);

		move = ResolveVelocity(move, info.Normal);
		environment = ResolveVelocity(environment, info.Normal);
		velocity = move + environment;
	}

	void SortCollisionLines(SequenceView<std::unique_ptr<CollisionLine>> lines, List<CollisionLine*>& ceilings, List<CollisionLine*>& grounds, List<CollisionLine*>& leftWalls, List<CollisionLine*>& rightWalls, float steepestSlope, float steepestCeiling)
	{
		ceilings.Clear();
		grounds.Clear();
		leftWalls.Clear();
		rightWalls.Clear();

		for (auto& line : lines)
		{
			if (line->Normal.Y >= steepestSlope)
			{
				grounds.Add(line.get());
			}
			else if (line->Normal.Y < steepestSlope && line->Normal.Y > -steepestCeiling)
			{
				if (line->Normal.X > 0.0f)
					leftWalls.Add(line.get());
				else if (line->Normal.X < 0.0f)
					rightWalls.Add(line.get());
			}
			else if (line->Normal.Y < -steepestCeiling)
			{
				ceilings.Add(line.get());
			}
		}
	}
}

void CharacterCollision::Reset()
{
	Ceilings.Clear();
	Grounds.Clear();
	LeftWalls.Clear();
	RightWalls.Clear();
}

void CharacterCollision::Initialize()
{
	SetCollisionPolygon(Character->Data->CollisionPolygon);
}

void CharacterCollision::Update(float elapsed)
{
	static constexpr int _maxIterations = 4;
	static List<CollisionLine*> _ceilings;
	static List<CollisionLine*> _grounds;
	static List<CollisionLine*> _leftWalls;
	static List<CollisionLine*> _rightWalls;

	Reset();

	auto velocity = Character->MoveVelocity + Character->EnvironmentVelocity;
	auto lines = Character->Node().CollisionLines.All();
	auto steepestSlope = Cosine(MaxSlopeAngle);
	auto steepestCeiling = Cosine(MaxCeilingAngle);

	SortCollisionLines(lines, _ceilings, _grounds, _leftWalls, _rightWalls, steepestSlope, steepestCeiling);

	for (auto i = 0; i < _maxIterations && IsGreaterThan(velocity.GetLengthSquared(), 0.0f) && IsGreaterThan(elapsed, 0.0f); i++)
	{
		if (Iterate(_ceilings, _grounds, _leftWalls, _rightWalls, velocity, elapsed))
			break;
	}

	Character->ProjectedPosition += velocity * elapsed;
}

auto CharacterCollision::Iterate(SequenceView<CollisionLine*> ceilings, SequenceView<CollisionLine*> grounds, SequenceView<CollisionLine*> leftWalls, SequenceView<CollisionLine*> rightWalls, Vector2& velocity, float& elapsed) -> bool
{
	auto collisionInfo = GetCollisionInfo(ceilings, grounds, leftWalls, rightWalls, _rigidPolygon, _normals, Character->ProjectedPosition, velocity * elapsed, Character->Depth, Character->ForceMiddle, elapsed);
	switch (collisionInfo.Type)
	{
		case CollisionType::LeftWall: ResolvePenetration(LeftWalls, collisionInfo, Character->ProjectedPosition, velocity, Character->MoveVelocity, Character->EnvironmentVelocity, elapsed); break;
		case CollisionType::RightWall: ResolvePenetration(RightWalls, collisionInfo, Character->ProjectedPosition, velocity, Character->MoveVelocity, Character->EnvironmentVelocity, elapsed); break;
		case CollisionType::Ground:	ResolvePenetration(Grounds, collisionInfo, Character->ProjectedPosition, velocity, Character->MoveVelocity, Character->EnvironmentVelocity, elapsed); break;
		case CollisionType::Ceiling: ResolvePenetration(Ceilings, collisionInfo, Character->ProjectedPosition, velocity, Character->MoveVelocity, Character->EnvironmentVelocity, elapsed); break;
		default: return true;
	}

	return false;
}

void CharacterCollision::SetCollisionPolygon(SequenceView<Vector2> polygon)
{
	auto left = std::numeric_limits<float>::max();
	auto right = std::numeric_limits<float>::min();
	auto bottom = std::numeric_limits<float>::max();
	auto top = std::numeric_limits<float>::min();

	_rigidPolygon = polygon;
	auto previous = polygon.Last();
	for (auto point : polygon)
	{
		if (point.X < left)
			left = point.X;

		if (point.X > right)
			right = point.X;

		if (point.Y < bottom)
			bottom = point.Y;

		if (point.Y > top)
			top = point.Y;

		auto side = point - previous;
		_normals.Add(side.GetNormal());
	}

	_size = { right - left, top - bottom };
}

auto CharacterCollision::GetCollisionBounds(Point2 position, Vector2 velocity) const -> Shapes::Rectangle
{
	return Shapes::Rectangle{ position.X - (_size.X * 0.5f), position.Y - Character->Data->GroundHeight, _size.X, _size.Y + Character->Data->GroundHeight }.Extended(velocity);
}
