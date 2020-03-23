#pragma once

#include "World/GameObject.h"

class StaticGameObject : public GameObject
{
public:
	StaticGameObject(WorldNode& node);

	auto Node() const -> WorldNode& override;
	auto GetBounds() const -> Rectangle override;

protected:
	Rectangle _bounds;

private:
	WorldNode& _node;
};

inline
auto StaticGameObject::Node() const -> WorldNode&
{
	return _node;
}

inline
auto StaticGameObject::GetBounds() const -> Rectangle
{
	return _bounds;
}
