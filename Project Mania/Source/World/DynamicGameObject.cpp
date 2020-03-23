#include "Pch.h"
#include "World/DynamicGameObject.h"
#include "Game/Game.h"

DynamicGameObject::DynamicGameObject(WorldNode& node) :
	_node(&node)
{
}

void DynamicGameObject::UpdateCurrentNode()
{
	if (!_node->Data.IsInside(ProjectedPosition))
	{
		auto& closest = Game()->World.Graph.FindClosestNode(Position, _node->Data.MapDepth);
		if (!_node->Data.IsSameAs(closest))
			ChangeNode(closest);
	}
}

void DynamicGameObject::ChangeNode(WorldNodeData& node)
{
	auto& previous = Node();
	_node = node.Node;

	ProjectedPosition = Game()->World.GetProjectedPosition(Position, node);

	OnNodeChanged(previous.Data);
}
