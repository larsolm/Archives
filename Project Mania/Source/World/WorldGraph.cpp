#include "Pch.h"
#include "World/WorldGraph.h"
#include "Game/Game.h"

void WorldGraph::Load()
{
	// Maybe separate each depth into it's own file.
	// Maybe make each depth it's own struct defining spindle and z values.

	auto file = ApplicationDirectory().GetFile("Assets/World.json");
	auto json = file.ReadText().Text;
	auto blueprint = Blueprint();
	
	auto stringReader = StringReader(json);
	stringReader.Read(blueprint, "json");

	auto blueprintReader = BlueprintReader(blueprint);
	blueprintReader.Read(_nodes);

	for (auto i = 0; i < _nodes.Count(); i++)
	{
		auto& nodes = _nodes.Item(i);
		for (auto n = 0; n < nodes.Count(); n++)
		{
			auto& node = nodes.ItemAtIndex(n);
			node.Name = nodes.GetKey(n).GetView();
			node.Index = n;
			node.MapDepth = i;
		}
	}

	for (auto& nodes : _nodes)
	{
		for (auto& node : nodes.Items())
			node.Initialize();
	}
}

void WorldGraph::LoadNode(WorldNodeData& node)
{
	if (node.Node == nullptr)
		CreateNode(node);

	for (auto& connection : node.Connections)
	{
		auto adjacent = GetNode(connection.ConnectionDepth, connection.ConnectionIndex);
		if (adjacent != nullptr && adjacent->Node == nullptr)
			CreateNode(*adjacent);
	}
}

void WorldGraph::UnloadNode(const WorldNodeData& node, const WorldNodeData& next)
{
	for (auto& connection : node.Connections)
	{
		if (!next.HasConnection(connection.ConnectionDepth, connection.ConnectionIndex))
		{
			auto previous = GetNode(connection.ConnectionDepth, connection.ConnectionIndex);
			if (previous != nullptr)
				DestroyNode(*previous);
		}
	}
}

auto WorldGraph::GetNode(StringView name) -> WorldNodeData*
{
	for (auto i = 0; i < _nodes.Count(); i++)
	{
		auto node = GetNode(i, name);
		if (node != nullptr)
			return node;
	}

	return nullptr;
}

auto WorldGraph::GetNode(int index) -> WorldNodeData*
{
	for (auto i = 0; i < _nodes.Count(); i++)
	{
		auto node = GetNode(i, index);
		if (node != nullptr)
			return node;
	}

	return nullptr;
}

auto WorldGraph::GetNode(int depth, StringView name) -> WorldNodeData*
{
	assert(depth >= 0 && depth < _nodes.Count());
	auto& nodes = _nodes.Item(depth);

	auto index = nodes.GetIndex(name);
	return index == Sequence::InvalidIndex ? nullptr : std::addressof(nodes.ItemAtIndex(index));
}

auto WorldGraph::GetNode(int depth, int index) -> WorldNodeData*
{
	assert(depth >= 0 && depth < _nodes.Count());
	auto& nodes = _nodes.Item(depth);

	return 	index < 0 || index >= nodes.Count() ? nullptr : std::addressof(nodes.ItemAtIndex(index));
}

auto WorldGraph::FindClosestNode(Point3 position, int depth) -> WorldNodeData&
{
	assert(depth >= 0 && depth < _nodes.Count());

	auto& nodes = _nodes.Item(depth);
	for (auto& node : nodes.Items())
	{
		if (node.IsInside(position))
			return node;
	}

	auto closest = 0;
	auto distance = std::numeric_limits<float>::max();
	for (auto i = 0; i < nodes.Count(); i++)
	{
		auto& node = nodes.ItemAtIndex(i);
		auto length = node.GetDistanceFrom(position);
		if (length < distance)
		{
			closest = i;
			distance = length;
		}
	}

	return nodes.ItemAtIndex(closest);
}

void WorldGraph::CreateNode(WorldNodeData& node)
{
	assert(node.Node == nullptr);

	auto& loaded = _loaded.Increment(std::make_unique<WorldNode>(node));
	loaded->Load();
}

void WorldGraph::DestroyNode(WorldNodeData& node)
{
	assert(node.Node != nullptr);

	node.Node->Unload();

	for (auto i = 0; i < _loaded.Count(); i++)
	{
		if (_loaded.Item(i)->Data.IsSameAs(node))
			_loaded.RemoveAt(i--);
	}
}
