#pragma once

#include "World/GameObject.h"

class DynamicGameObject : public GameObject
{
public:
	DynamicGameObject(WorldNode& node);

	Point3 Position;
	Point3 PreviousPosition;
	Point2 ProjectedPosition;

	auto Node() const -> WorldNode& override;

	void UpdateCurrentNode();
	void ChangeNode(WorldNodeData& node);

private:
	WorldNode* _node;

	virtual void OnNodeChanged(const WorldNodeData& previous) {};
};

inline
auto DynamicGameObject::Node() const -> WorldNode&
{
	return *_node;
}
