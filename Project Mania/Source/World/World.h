#pragma once

#include "World/CollisionLine.h"
#include "World/Projectile.h"
#include "World/WorldGraph.h"
#include "Character/Character.h"

#include <Pargon.h>
#include <PargonShapes.h>

using namespace Pargon;
using namespace Pargon::Shapes;

class Player;
class WorldNode;
class WorldNodeData;

class World
{
public:
	static constexpr Vector3 XAxis = { 1.0f, 0.0f, 0.0f };
	static constexpr Vector3 YAxis = { 0.0f, 1.0f, 0.0f };
	static constexpr Vector3 ZAxis = { 0.0f, 0.0f, 1.0f };

	WorldGraph Graph;

	Character::Storage Characters;
	Projectile::Storage Projectiles;

	Player* Player;

	void Load();
	void Update(Time elapsed);

	auto GetProjectedPosition(Point3 position, const WorldNodeData& node) const -> Point2;
	auto GetRealPosition(Point2 position, const WorldNodeData& node) const -> Point3;

	auto CreateCharacter(WorldNode& node, CharacterData& data, Point2 position) -> Character&;
	void DestroyCharacter(Character& character);

	auto CreateProjectile(WorldNode& node, Point2 position, Angle rotation) -> Projectile&;
	void DestroyProjectile(Projectile& projectile);

	void ChangeNode(const WorldNodeData& from, WorldNodeData& to);

private:
	struct PendingDestroy
	{
		uint64_t Id;
		void* Storage;
		void(*Destroy)(uint64_t id, void* storage);
	};

	List<Character::IdType> _charactersToDestroy;
	List<Projectile::IdType> _projectilesToDestroy;
};
