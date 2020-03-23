#include "Pch.h"
#include "Character/Character.h"
#include "World/DepthTrigger.h"
#include "Game/Game.h"

void DepthTrigger::Initialize()
{
	Trigger::Initialize();

	auto previous = Polygon.Vertices().Last();
	for (auto current : Polygon.Vertices())
	{
		Game()->Rendering.CreateDebugModel({ { previous.X, previous.Y }, { current.X, current.Y } }, Node().Data.IsSpindle, Node().Data.WorldAngle, Node().Data.ZPosition, Yellow);
		previous = current;
	}
}

void DepthTrigger::Execute(Character& character)
{
	character.CanMoveToBack = character.CanMoveToBack || CanMoveBackward;
	character.CanMoveToMiddle = character.CanMoveToMiddle || CanMoveMiddle;
	character.CanMoveToFront = character.CanMoveToFront || CanMoveForward;
	character.ForceMiddle = character.ForceMiddle || ForceMiddle;
}

void DepthTrigger::FromBlueprint(BlueprintReader& reader)
{
	List<Point2> vertices;

	reader.ReadChild("CanMoveBackward", CanMoveBackward);
	reader.ReadChild("CanMoveMiddle", CanMoveMiddle);
	reader.ReadChild("CanMoveForward", CanMoveForward);
	reader.ReadChild("ForceMiddle", ForceMiddle);
	reader.ReadChild("Polygon", vertices);

	for (auto vertex : vertices)
		Polygon.AppendVertex(vertex);
}
