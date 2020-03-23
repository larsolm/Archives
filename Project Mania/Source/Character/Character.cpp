#include "Pch.h"
#include "Character/Character.h"
#include "Game/Game.h"

void Character::Initialize(Point2 position)
{
	ProjectedPosition = position;
	Position = Game()->World.GetRealPosition(position, Node().Data);
	PreviousPosition = Position;

	Chassis.Data = Data->Chassis;
	Controller = Controller::ControllerMap.CreateData(Data->Controller);
	Processor = Processor::ProcessorMap.CreateData(Data->Processor);

	Collision.Character = this;
	Chassis.Character = this;
	Controller->Character = this;
	Processor->Character = this;

	Collision.Initialize();
	Chassis.Initialize();
	Controller->Initialize();
	Processor->Initialize();
}

void Character::Update(float elapsed)
{
	PreviousPosition = Position;

	Controller->Update(elapsed);
	Chassis.Update(elapsed);
	Processor->Update(elapsed);

	Position = Game()->World.GetRealPosition(ProjectedPosition, Node().Data);
}

void Character::Interpolate(float interpolation)
{
	Chassis.Skeleton->Interpolate(interpolation);
}

void Character::Animate(float elapsed)
{
	Chassis.Skeleton->Animate(elapsed);
}

void Character::OnNodeChanged(const WorldNodeData& previous)
{
	auto angleChange = previous.WorldAngle - Node().Data.WorldAngle;

	EnvironmentVelocity.Rotate(angleChange);
	MoveVelocity.Rotate(angleChange);

	ProjectedPosition = Game()->World.GetProjectedPosition(Position, Node().Data);

	Processor->NodeChanged(previous);
}
