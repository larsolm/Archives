#pragma once

#include "Pargon/Game/Core/GameCore.h"
#include "Pargon/Types/Types.h"
#include "Pargon/Math/Math.h"

namespace SuperMarioWar
{
	struct CollisionBox;

	enum class InputType
	{
		Controller,
		Keyboard,
		AI
	};

	enum class PlayerState
	{
		Normal,
		Spawned,
		Dead
	};

	class Player : public Pargon::Game::Core::Component
	{
	public:
		Pargon::Color TeamColor;
		InputType InputType = InputType::Controller;
		PlayerState State = PlayerState::Normal;
		float Invincibility = 0.0f;
		float Respawn = 0.0f;
		int Score = 0;
		int PlayerIndex = 0;
		int ControllerIndex = 0;
		Pargon::Math::Vector2 Velocity;
		bool OnGround = false;
		bool WantsJump = false;
		bool WantsFallthrough = false;
		bool WantsTurbo = false;
		float HorizontalMoveVector = 0;
		int JumpTime = 0;
		const int JumpThreashold = 10;
		float AnimationTimer = 0;

		float AnimationSpeed = 10;
		float TurboSpeed = 120;
		float WalkSpeed = 80;
		float Agility = 5;
		float Gravity = 5;
		float TerminalVelocity = 250;
		float AirResistance = 0.95f;
		float AirControl = 2;
		float HangTime = 7;
		float JumpImpulse = 80;
		float Traction = 1;

		Pargon::String JumpSound;
		Pargon::String BonkSound;
		Pargon::String DeathSound;
		Pargon::String SkidSound;
		Pargon::String StompSound;

		const CollisionBox* Ground = nullptr;
		Pargon::List<const CollisionBox*> OneWays;

		int Kills = 0;
		int Deaths = 0;
	};
}

template<> void Pargon::CreateMembers<SuperMarioWar::Player>(Pargon::Reflection::Type* type);
