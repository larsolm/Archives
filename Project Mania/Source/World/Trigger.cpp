#include "Pch.h"
#include "World/Trigger.h"

void Trigger::Initialize()
{
	auto left = std::numeric_limits<float>::max();
	auto right = std::numeric_limits<float>::min();
	auto bottom = std::numeric_limits<float>::max();
	auto top = std::numeric_limits<float>::min();

	for (auto point : Polygon.Vertices())
	{
		if (point.X < left)
			left = point.X;

		if (point.X > right)
			right = point.X;

		if (point.Y < bottom)
			bottom = point.Y;

		if (point.Y > top)
			top = point.Y;
	}

	_bounds = { left, bottom, right - left, top - bottom };
}
