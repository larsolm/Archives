#include "Pch.h"
#include "World/CollisionLine.h"
#include "Game/Game.h"

void CollisionLine::Initialize()
{
	CalculateNormal();

	auto left = Minimum(Line.Start.X, Line.End.X);
	auto right = Maximum(Line.Start.X, Line.End.X);
	auto bottom = Minimum(Line.Start.Y, Line.End.Y);
	auto top = Maximum(Line.Start.Y, Line.End.Y);

	_bounds.X = left;
	_bounds.Y = bottom;
	_bounds.Width = right - left;
	_bounds.Height = top - bottom;

	// DEBUG
	auto color = Depth == CollisionDepth::Back ? Purple : Depth == CollisionDepth::Front ? Blue : Black;
	Game()->Rendering.CreateDebugModel(Line, Node().Data.IsSpindle, Node().Data.WorldAngle, Node().Data.ZPosition, color);
}

void CollisionLine::CalculateNormal()
{
	Normal = Line.GetNormal();
	Slope = IsCloseTo(Line.Start.X, Line.End.X) ? std::numeric_limits<float>::max() : Line.GetSlope();
}

void CollisionLine::FlipNormal()
{
	auto start = Line.Start;
	Line.Start = Line.End;
	Line.End = start;

	CalculateNormal();
}

void CollisionLine::FromBlueprint(BlueprintReader& reader)
{
	reader.ReadChild("Start", Line.Start);
	reader.ReadChild("End", Line.End);
	reader.ReadChild("Depth", Depth);
}
