#pragma once

#include <Pargon.h>
#include <PargonShapes.h>

#include "World/CollisionLine.h"
#include "World/DepthTrigger.h"
#include "World/SpindleTrigger.h"

#include "World/Storages/PermanentStaticallyTypedStorage.h"
#include "World/Storages/PermanentDynamicallyTypedStorage.h"

using namespace Pargon;
using namespace Pargon::Shapes;

class WorldNode;

struct WorldConnectionData
{
	int ConnectionDepth;
	int ConnectionIndex;
	String ConnectionName;

	void FromBlueprint(BlueprintReader& reader);
};

class WorldNodeData
{
public:
	WorldNode* Node = nullptr;

	StringView Name;
	int Index;

	bool IsSpindle;
	bool InvertCamera;

	Angle WorldAngle;

	float GravityStrength;
	float ZPosition;

	int MapDepth;
	int MapHeight;

	Polygon2 Polygon;
	Polygon2 MapProjectionPolygon;

	List<WorldConnectionData> Connections;

	void Initialize();
	auto IsSameAs(const WorldNodeData& node) const -> bool;
	auto IsInside(Point2 position) const -> bool;
	auto IsInside(Point3 position) const -> bool;
	auto GetDistanceFrom(Point3 position) const -> float;
	auto HasConnection(int depth, int index) const -> bool;
	auto GetCameraAngle() const -> Quaternion;

	void FromBlueprint(BlueprintReader& reader);

private:
	void ResolveConnections();
	void ResolveMapProjectionPolygon();
	auto GetMapPosition(Point2 position) const -> Point2;
};

class WorldNode
{
public:
	WorldNode(WorldNodeData& data);
	~WorldNode();

	WorldNodeData& Data;

	CollisionLine::Storage CollisionLines;
	SpindleTrigger::Storage SpindleTriggers;
	DepthTrigger::Storage DepthTriggers;

	void Load();
	void Unload();
};
