#include "Pch.h"
#include "World/WorldNode.h"
#include "Game/Game.h"

void WorldNodeData::Initialize()
{
	ResolveConnections();
	ResolveMapProjectionPolygon();
}

auto WorldNodeData::IsSameAs(const WorldNodeData& other) const -> bool
{
	return MapDepth == other.MapDepth && Index == other.Index;
}

auto WorldNodeData::IsInside(Point2 position) const -> bool
{
	return Shapes::IsContained(position, Polygon);
}

auto WorldNodeData::IsInside(Point3 position) const -> bool
{
	auto projected = Game()->World.GetProjectedPosition(position, *this);
	auto map = GetMapPosition(projected);
	return Shapes::IsContained(map, MapProjectionPolygon);
}

auto WorldNodeData::GetDistanceFrom(Point3 position) const -> float
{
	auto projected = Game()->World.GetProjectedPosition(position, *this);
	auto point = Shapes::GetClosestPoint(projected, MapProjectionPolygon);
	return (projected - point).GetLengthSquared();
}

auto WorldNodeData::HasConnection(int depth, int index) const -> bool
{
	if (depth == MapDepth && index == Index)
		return true;

	for (auto& connection : Connections)
	{
		if (connection.ConnectionDepth == depth && connection.ConnectionIndex == index)
			return true;
	}

	return false;
}

auto WorldNodeData::GetCameraAngle() const -> Quaternion
{
	auto yaw = IsSpindle ? (InvertCamera ? 90_degrees : 270_degrees) : (InvertCamera ? 180_degrees : 0.0_radians);
	auto pitch = 0.0_radians;
	auto roll = WorldAngle;
	return Quaternion::CreateFromEulerAngles(yaw, pitch, roll);
}

void WorldNodeData::ResolveMapProjectionPolygon()
{
	assert(MapProjectionPolygon.Vertices().IsEmpty());

	for (auto point : Polygon.Vertices())
	{
		auto position = GetMapPosition(point);
		MapProjectionPolygon.AppendVertex(position);
	}
}

void WorldNodeData::ResolveConnections()
{
	for (auto& connection : Connections)
	{
		auto node = Game()->World.Graph.GetNode(connection.ConnectionName);
		if (node != nullptr)
		{
			connection.ConnectionDepth = node->MapDepth;
			connection.ConnectionIndex = node->Index;
		}
	}
}

auto WorldNodeData::GetMapPosition(Point2 position) const -> Point2
{
	static constexpr float _tallestNode = 1000.0f;
	static constexpr float _widestNode = 1000.0f;

	return
	{
		position.X + (WorldAngle.InDegrees() * _widestNode),
		position.Y + (_tallestNode * MapHeight)
	};
}

void WorldNodeData::FromBlueprint(BlueprintReader& reader)
{
	List<Point2> polygon;

	reader.ReadChild("IsSpindle", IsSpindle);
	reader.ReadChild("InvertCamera", InvertCamera);
	reader.ReadChild("WorldAngle", WorldAngle);
	reader.ReadChild("GravityStrength", GravityStrength);
	reader.ReadChild("ZPosition", ZPosition);
	reader.ReadChild("MapHeight", MapHeight);
	reader.ReadChild("Polygon", polygon);
	reader.ReadChild("Connections", Connections);

	Polygon.SetVertices(polygon);
}

void WorldConnectionData::FromBlueprint(BlueprintReader& reader)
{
	reader.Read(ConnectionName);
}

WorldNode::WorldNode(WorldNodeData& data) :
	Data(data)
{
	data.Node = this;
}

WorldNode::~WorldNode()
{
	Data.Node = nullptr;
}

namespace
{
	template<typename Storage>
	void ReadObjects(StringView name, Storage& storage, WorldNode& node, BlueprintReader& reader)
	{
		if (reader.MoveDown(name))
		{
			for (auto valid = reader.FirstChild(); valid; valid = reader.NextChild())
			{
				auto object = storage.Create(node);
				object->FromBlueprint(reader);
				object->Initialize();
			}

			reader.MoveUp();
		}
	}
}

void WorldNode::Load()
{
	auto file = ApplicationDirectory().GetFile(FormatString("Assets/Nodes/{}.json", Data.Name));
	auto json = file.ReadText().Text;
	auto blueprint = Blueprint();
	auto stringReader = StringReader(json);
	stringReader.Read(blueprint, "json");

	auto blueprintReader = BlueprintReader(blueprint);

	ReadObjects("CollisionLines", CollisionLines, *this, blueprintReader);
	ReadObjects("SpindleTriggers", SpindleTriggers, *this, blueprintReader);
	ReadObjects("DepthTriggers", DepthTriggers, *this, blueprintReader);
}

void WorldNode::Unload()
{
}
