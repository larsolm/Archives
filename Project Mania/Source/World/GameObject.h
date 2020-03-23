#pragma once

#include <Pargon.h>
#include <PargonShapes.h>

using namespace Pargon;
using namespace Pargon::Shapes;

class WorldNode;
class WorldNodeData;

class GameObject
{
public:
	virtual auto Node() const -> WorldNode& = 0;
	virtual auto GetBounds() const -> Rectangle = 0;
};
