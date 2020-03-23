#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/PlayerManager.h"
#include "SuperMarioWar/Tilemap/Tilemap.h"

using namespace Pargon;
using namespace SuperMarioWar;

void PlayerManager::StartGame()
{
	auto camera = _cameras->ObjectNamed("Camera");
	auto dimentions = camera->GetDimensions(Globals().Window.Width(), Globals().Window.Height());
	_worldMinX = -dimentions.X() / 2;
	_worldMaxX = dimentions.X() / 2;
	_worldMinY = -dimentions.Y() / 2;
	_worldMaxY = dimentions.Y() / 2;
}

void PlayerManager::UpdatePlayers(float elapsed)
{
	for (auto player : _players->AllObjects())
	{
		if (player->State == PlayerState::Spawned)
		{
			player->Invincibility -= elapsed;
			if (player->Invincibility < 0.0f)
				player->State = PlayerState::Normal;
		}

		ProcessInput(player);
		MovePlayer(player, elapsed);
		AnimatePlayer(player, elapsed);
	}
}

void PlayerManager::ProcessInput(Player* player)
{
	if (player->State == PlayerState::Dead)
		return;

	if (player->InputType == InputType::Controller)
	{
		auto controller = Globals().Controller.get();

		player->HorizontalMoveVector = controller->CurrentState(player->ControllerIndex).LeftStick.X();
		player->WantsJump = controller->IsButtonDown(player->ControllerIndex, Input::ControllerButton::A);
		player->WantsTurbo = controller->IsButtonDown(player->ControllerIndex, Input::ControllerButton::X);
		player->WantsFallthrough = controller->CurrentState(player->ControllerIndex).LeftStick.Y() < -0.5f;
	}
	else if (player->InputType == InputType::Keyboard)
	{
		auto keyboard = Globals().Keyboard.get();

		player->HorizontalMoveVector = (keyboard->IsKeyDown(Key::Right) ? 1.0f : 0.0f) - (keyboard->IsKeyDown(Key::Left) ? 1.0f : 0.0f);
		player->WantsJump = keyboard->IsKeyDown(Key::Space);
		player->WantsTurbo = keyboard->IsKeyDown(Key::LeftShift);
		player->WantsFallthrough = keyboard->IsKeyDown(Key::Down);
	}
}

namespace
{
	float GetAcceleration(float velocity, float moveVector, float friction, float agility)
	{
		if (abs(moveVector) > 0)
			return velocity + (agility * friction * moveVector);
		else
		{
			int dir = velocity >= 0 ? 1 : -1;

			velocity -= (agility * friction * dir);
			if (abs(velocity) <= (agility * friction))
				return 0;

			return velocity;
		}
	}
}

void PlayerManager::MovePlayer(Player* player, float elapsed)
{
	auto spatial = player->Entity()->GetComponent<Game::X2d::Spatial>();

	if (player->State == PlayerState::Dead)
	{
		player->Velocity.Y() -= player->Gravity;

		if (player->Velocity.Y() < -player->TerminalVelocity)
			player->Velocity.Y() = -player->TerminalVelocity;

		spatial->Position += player->Velocity * elapsed;
		return;
	}

	if (player->OnGround)
	{
		auto friction = player->Traction; // * Ground->Friction;

		if (friction < 0)
			friction = 0;
		else if (friction > 1)
			friction = 1;

		player->Velocity.X() = GetAcceleration(player->Velocity.X(), player->HorizontalMoveVector, friction, player->Agility);
	}
	else
	{
		player->Velocity.X() = GetAcceleration(player->Velocity.X(), player->HorizontalMoveVector, player->AirResistance, player->AirControl);
	}

	auto maxSpeed = player->WantsTurbo ? player->TurboSpeed : player->WalkSpeed;
	 
	if (player->Velocity.X() < -maxSpeed)
		player->Velocity.X() += player->Agility;
	else if (player->Velocity.X() > maxSpeed)
		player->Velocity.X() -= player->Agility;

	player->Velocity.Y() -= player->Gravity;

	if (player->Velocity.Y() < -player->TerminalVelocity)
		player->Velocity.Y() = -player->TerminalVelocity;

	if (player->OnGround) 
		player->JumpTime = 0;
	 
	if (player->WantsJump)
	{
		if (player->JumpTime == 0)
		{
			if (player->OnGround && player->WantsFallthrough && player->Ground->OneWay)
			{
				player->OneWays.Add(player->Ground);
				player->JumpTime = player->JumpThreashold;
			}
			else if (player->OnGround)
			{
				auto sound = _sounds->ObjectNamed(player->JumpSound);
				if (sound != nullptr)
					Globals().Audio->PlaySound(**sound, false, 1.0f);

				player->JumpTime++;
				player->Velocity.Y() += player->JumpImpulse;
			}
		}
		else if (player->JumpTime > 0 && player->JumpTime <= player->JumpThreashold)
		{
			player->JumpTime++;
			player->Velocity.Y() += player->HangTime;
		}
	}
	else
	{
		player->JumpTime = player->JumpThreashold;
	}

	spatial->Position += player->Velocity * elapsed;

	if (spatial->Position.X() > (_worldMaxX + (spatial->Size.X()) / 2))
		spatial->Position.X() = (_worldMinX - (spatial->Size.X() / 2));
	else if (spatial->Position.X() < (_worldMinX - (spatial->Size.X() / 2)))
		spatial->Position.X() = (_worldMaxX + (spatial->Size.X() / 2));
}

void PlayerManager::AnimatePlayer(Player* player, float elapsed)
{
	auto spatial = player->Entity()->GetComponent<Game::X2d::Spatial>();
	auto shape = player->Entity()->GetComponent<Game::X2d::ShapeInstance>();

	if (player->State == PlayerState::Dead)
	{
		shape->Frame = 4;
		return;
	}

	shape->Color = player->State == PlayerState::Spawned ? Color::Green() : Color::White();

	if ((player->Velocity.X() < 0 && !spatial->FlippedHorizontally) || (player->Velocity.X() > 0 && spatial->FlippedHorizontally))
		spatial->FlippedHorizontally = !spatial->FlippedHorizontally;

	if (player->OnGround)
	{
		if ((player->Velocity.X() < 0 && player->HorizontalMoveVector > 0) || (player->Velocity.X() > 0 && player->HorizontalMoveVector < 0))
		{
			if (shape->Frame != 3)
			{
				auto sound = _sounds->ObjectNamed(player->SkidSound);
				Globals().Audio->PlaySound(**sound, false, 1.0f);
			}

			shape->Frame = 3;
		}
		else
		{
			auto abs = Math::AbsoluteValue<float>(player->Velocity.X());
			
			if (abs > 0)
			{
				player->AnimationTimer += elapsed * player->AnimationSpeed;
				if (player->AnimationTimer >= 2)
					player->AnimationTimer -= 2;
			}
			else
				player->AnimationTimer = 0;

			shape->Frame = player->AnimationTimer > 1 ? 1 : 0;
		}
	}
	else
	{
		shape->Frame = 2;
	}
}
