#include "Pch.h"
#include "Character/Characters/Player.h"
#include "World/SpindleTrigger.h"
#include "Game/Game.h"

void SpindleTrigger::Initialize()
{
	Game()->Rendering.CreateDebugModel(_bounds, Node().Data.IsSpindle, Node().Data.WorldAngle, Node().Data.ZPosition, Green);
}

void SpindleTrigger::Interact(Player& player)
{
	assert(Connection != nullptr);

	player.ChangeNode(*Connection);
}

void SpindleTrigger::FromBlueprint(BlueprintReader& reader)
{
	String connection;

	reader.ReadChild("X", _bounds.X);
	reader.ReadChild("Y", _bounds.Y);
	reader.ReadChild("Width", _bounds.Width);
	reader.ReadChild("Height", _bounds.Height);
	reader.ReadChild("Connection", connection);

	Connection = Game()->World.Graph.GetNode(connection);
}
