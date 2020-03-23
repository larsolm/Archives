#include "Pch.h"
#include "Character/Characters/Player.h"
#include "World/World.h"
#include "Game/Game.h"

void World::Load()
{
	Graph.Load();

	// Temporary loading for debug.
	auto node = Graph.GetNode("Bottom");
	auto player = Game()->Data.GetCharacterData("Player");
	auto enemy = Game()->Data.GetCharacterData("Enemy");

	Graph.LoadNode(*node);
	Player = static_cast<class Player*>(&CreateCharacter(*node->Node, *player, { 0.0f, -5.0f }));
	//CreateCharacter(*node->Node, *enemy, { 2.0f, -10.0f });
}

namespace
{
	template<typename T, typename StorageType>
	void DestroyGameObjects(List<T>& ids, StorageType& storage)
	{
		for (auto id : ids)
			storage.Destroy(id);

		ids.Clear();
	}
}

void World::Update(Time elapsed)
{
	auto time = static_cast<float>(elapsed.InSeconds());

	for (auto& character : Characters.All())
	{
		character->UpdateCurrentNode();
		character->Update(time);
	}

	for (auto& projectile : Projectiles.All())
	{
		projectile->UpdateCurrentNode();
		projectile->Update(time);
	}

	DestroyGameObjects(_charactersToDestroy, Characters);
	DestroyGameObjects(_projectilesToDestroy, Projectiles);
}

auto World::GetProjectedPosition(Point3 position, const WorldNodeData& node) const -> Point2
{
	if (node.IsSpindle)
	{
		auto x = position.Z;
		auto y = -SquareRoot(position.X * position.X + position.Y * position.Y);
		return { x, y };
	}
	else
	{
		auto axis = Vector2{ position.X, position.Y }.Rotated(-node.WorldAngle);
		return { axis.X, axis.Y };
	}
}

auto World::GetRealPosition(Point2 position, const WorldNodeData& node) const -> Point3
{
	if (node.IsSpindle)
	{
		auto x = -Sine(node.WorldAngle) * position.Y;
		auto y = Cosine(node.WorldAngle) * position.Y;
		auto z = position.X;
		return { x, y, z };
	}
	else
	{
		auto axis = Vector2{ position.X, position.Y }.Rotated(node.WorldAngle);
		return { axis.X, axis.Y, node.ZPosition };
	}
}

auto World::CreateCharacter(WorldNode& node, CharacterData& data, Point2 position) -> Character&
{
	auto character = Characters.Create(data, node);
	character->Initialize(position);
	character->Start();
	return *character;
}

void World::DestroyCharacter(Character& character)
{
	character.Stop();
	_charactersToDestroy.Add(character.Id);
}

auto World::CreateProjectile(WorldNode& node, Point2 position, Angle rotation) -> Projectile&
{
	auto projectile = Projectiles.Create(node);
	projectile->Initialize(position, rotation);
	projectile->Start();
	return *projectile;
}

void World::DestroyProjectile(Projectile& projectile)
{
	projectile.Stop();
	_projectilesToDestroy.Add(projectile.Id);
}

void World::ChangeNode(const WorldNodeData& from, WorldNodeData& to)
{
	Graph.UnloadNode(from, to);
	Graph.LoadNode(to);
}
