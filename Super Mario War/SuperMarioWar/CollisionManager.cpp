#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/CollisionManager.h"
#include "SuperMarioWar/GameManager.h"

using namespace Pargon;
using namespace SuperMarioWar;

namespace
{
	struct Rectangle
	{
		float X, Y, Width, Height;
	};

	auto GetBounds(Game::X2d::Spatial* spatial) -> Rectangle
	{
		auto transform = Math::Matrix4x4::CreateIdentity();
		spatial->ApplyTransform(transform);

		Rectangle bounds;
		bounds.X = transform.GetTranslation().X();
		bounds.Y = transform.GetTranslation().Y();
		bounds.Width = transform.GetScale().X();
		bounds.Height = transform.GetScale().Y();

		if (spatial->FlippedHorizontally)
			bounds.X -= bounds.Width;

		if (spatial->FlippedVertically)
			bounds.Y -= bounds.Height;

		return bounds;
	}

	auto IsBetween(float number, float left, float right) -> bool
	{
		return number > left && number < right;
	}

	auto GetPenetration(float number1, float number2, float left, float right) -> float
	{
		auto penetration1 = IsBetween(number1, left, right) ? number1 - right : 0.0f;
		auto penetration2 = IsBetween(number2, left, right) ? left - number2 : 0.0f;

		return penetration2 < penetration1 ? penetration2 : -penetration1;
	}

	void ResolvePlayer(SuperMarioWar::Player* player, Game::X2d::Spatial* spatial, const Rectangle& box, const Rectangle& collision, const CollisionBox& collider, List<const CollisionBox*>& currentColliders, Audio::Sound& sound)
	{
		auto left1 = box.X;
		auto right1 = box.X + box.Width;
		auto top1 = box.Y + box.Height;
		auto bottom1 = box.Y;

		auto left2 = collision.X;
		auto right2 = collision.X + collision.Width;
		auto top2 = collision.Y + collision.Height;
		auto bottom2 = collision.Y;

		auto betweenVertical = IsBetween(top1, bottom2, top2) || IsBetween(bottom1, bottom2, top2);
		auto betweenHorizontal = IsBetween(left1, left2, right2) || IsBetween(right1, left2, right2);

		if (!betweenVertical || !betweenHorizontal)
			return;

		currentColliders.Add(&collider);

		if (player->OneWays.Contains(&collider))
			return;

		if (collider.OneWay && player->Velocity.Y() >= 0.0f)
		{
			player->OneWays.Add(&collider);
			return;
		}

		auto horizontalPenetration = GetPenetration(left1, right1, left2, right2);
		auto verticalPenetration = GetPenetration(bottom1, top1, bottom2, top2);

		if (Math::AbsoluteValue(horizontalPenetration) < Math::AbsoluteValue(verticalPenetration))
		{
			if (collider.OneWay)
			{
				player->OneWays.Add(&collider);
				return;
			}

			if (player->Velocity.X() * horizontalPenetration < 0.0f)
				player->Velocity.X() = 0.0f;

			spatial->Position.X() += horizontalPenetration;
		}
		else
		{
			if (player->Velocity.Y() * verticalPenetration < 0.0f)
				player->Velocity.Y() = 0.0f;

			if (verticalPenetration > 0.0f)
			{
				player->Ground = &collider;
				player->OnGround = true;
			}
			else
			{
				Globals().Audio->PlaySound(sound, false, 1.0f);
			}

			spatial->Position.Y() += verticalPenetration;
		}
	}
}

void CollisionManager::ResolveTilemaps(float elapsed)
{
	for (auto player : _players->AllObjects())
	{
		if (player->State == PlayerState::Dead)
			continue;

		List<const CollisionBox*> currentColliders;

		auto spatial = player->Entity()->GetComponent<Game::X2d::Spatial>();
		auto playerBounds = GetBounds(spatial);
		player->OnGround = false;
		player->Ground = nullptr;

		for (auto tilemap : _tilemaps->AllObjects())
		{
			auto& collisions = (*tilemap->_tilemap)->Collisions();
			auto tileSpatial = tilemap->Entity()->GetComponent<Game::X2d::Spatial>();
			auto bounds = GetBounds(tileSpatial);

			for (auto& collision : collisions)
			{
				Rectangle box;
				box.X = bounds.X + (collision.X * bounds.Width);
				box.Y = bounds.Y + (collision.Y * bounds.Height);
				box.Width = collision.Width * bounds.Width;
				box.Height = collision.Height * bounds.Height;

				auto sound = _sounds->ObjectNamed(player->BonkSound);
				ResolvePlayer(player, spatial, playerBounds, box, collision, currentColliders, **sound);
			}
		}

		for (auto i = 0; i < player->OneWays.Count(); i++)
		{
			if (!currentColliders.Contains(player->OneWays[i]))
				player->OneWays.RemoveAt(i--);
		}
	}
}

namespace
{
	auto IsSameOrBetween(float number, float left, float right) -> bool
	{
		return number >= left && number <= right;
	}

	auto GetPlayerPenetration(float number1, float number2, float left, float right) -> float
	{
		auto penetration1 = IsSameOrBetween(number1, left, right) ? number1 - right : 0.0f;
		auto penetration2 = IsSameOrBetween(number2, left, right) ? left - number2 : 0.0f;

		return penetration2 < penetration1 ? penetration2 : -penetration1;
	}

	void ResolveStomp(GameManager* game, SuperMarioWar::Player* player1, Game::X2d::Spatial* spatial1, SuperMarioWar::Player* player2, Game::X2d::Spatial* spatial2)
	{
		auto box1 = GetBounds(spatial1);
		auto box2 = GetBounds(spatial2);

		auto left1 = box1.X;
		auto right1 = box1.X + box1.Width;
		auto top1 = box1.Y + box1.Height;
		auto bottom1 = box1.Y;

		auto left2 = box2.X;
		auto right2 = box2.X + box2.Width;
		auto top2 = box2.Y + box2.Height;
		auto bottom2 = box2.Y;

		auto betweenVertical = IsSameOrBetween(top1, bottom2, top2) || IsSameOrBetween(bottom1, bottom2, top2);
		auto betweenHorizontal = IsSameOrBetween(left1, left2, right2) || IsSameOrBetween(right1, left2, right2);

		if (!betweenVertical || !betweenHorizontal)
			return;

		auto horizontalPenetration = GetPlayerPenetration(left1, right1, left2, right2);
		auto verticalPenetration = GetPlayerPenetration(bottom1, top1, bottom2, top2);

		if (Math::AbsoluteValue(horizontalPenetration) <= Math::AbsoluteValue(verticalPenetration))
		{
			spatial1->Position.X() += horizontalPenetration * 0.5f;
			spatial2->Position.X() -= horizontalPenetration * 0.5f;

			if (player1->Velocity.X() * horizontalPenetration < 0.0f)
				player1->Velocity.X() = -player1->Velocity.X();
			
			if (player2->Velocity.X() * horizontalPenetration > 0.0f)
				player2->Velocity.X() = -player2->Velocity.X();
		}
		else
		{
			if (verticalPenetration > 0.0f)
			{
				player1->JumpTime = 1;
				player1->Velocity.Y() = player1->JumpImpulse;

				if (player2->State == PlayerState::Normal)
					game->KillPlayer(player2, player1);
			}
			else
			{
				player2->JumpTime = 1;
				player2->Velocity.Y() = player2->JumpImpulse;

				if (player1->State == PlayerState::Normal)
					game->KillPlayer(player1, player2);
			}
		}
	}
}

void CollisionManager::ResolveStomps(float elapsed, GameManager* game)
{
	auto players = _players->AllObjects();
	for (auto i = 0; i < players.Count(); i++)
	{
		auto player1 = players[i];
		auto spatial1 = player1->Entity()->GetComponent<Game::X2d::Spatial>();

		if (player1->State == PlayerState::Dead)
			continue;

		for (auto j = i + 1; j < players.Count(); j++)
		{
			auto player2 = players[j];
			auto spatial2 = player2->Entity()->GetComponent<Game::X2d::Spatial>();

			if (player2->State == PlayerState::Dead)
				continue;

			ResolveStomp(game, player1, spatial1, player2, spatial2);
		}
	}
}
