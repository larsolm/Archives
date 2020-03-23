#pragma once

#include "Data/Data.h"

class Character;
class Projectile;
class WorldNodeData;

class Processor
{
public:
	static DataMap<Processor> ProcessorMap;

	Character* Character;

	virtual void Initialize() {}
	virtual void Update(float elapsed) {}
	virtual void ProjectileHit(Projectile& projectile) {};
	virtual void NodeChanged(const WorldNodeData& previous) {}

protected:
	void UpdateDepth(float elapsed);
	void ApplyDamage(float amount);

	virtual void OnShieldDepleted() {};
	virtual void OnDeath() {};
};
