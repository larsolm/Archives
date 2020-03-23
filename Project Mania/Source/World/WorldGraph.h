#pragma once

#include "World/WorldNode.h"

#include <Pargon.h>

using namespace Pargon;

class WorldGraph
{
public:
	void Load();

	void LoadNode(WorldNodeData& node);
	void UnloadNode(const WorldNodeData& node, const WorldNodeData& next);

	auto GetNode(StringView name) -> WorldNodeData*;
	auto GetNode(int index) -> WorldNodeData*;

	auto GetNode(int depth, StringView name) -> WorldNodeData*;
	auto GetNode(int depth, int index) -> WorldNodeData*;

	auto FindClosestNode(Point3 position, int depth) -> WorldNodeData&;

private:
	List<Map<String, WorldNodeData>> _nodes;
	List<std::unique_ptr<WorldNode>> _loaded;

	void CreateNode(WorldNodeData& node);
	void DestroyNode(WorldNodeData& node);
};
