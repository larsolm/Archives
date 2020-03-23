#pragma once

#include "Character/Bone.h"
#include "Data/SkeletonData.h"

#include <Pargon.h>
#include <PargonScene.h>

using namespace Pargon;

class Character;

class Skeleton
{
public:
	Character* Character;
	SkeletonData* Data;

	Bone Root;
		
	virtual void Initialize();
	virtual void Interpolate(float interpolation);
	virtual void Animate(float elapsed);

	auto GetBone(StringView name) -> Bone&;

protected:
	void ComputeIkChain(SequenceView<Bone*> chain, Bone* end, Point3 targetPosition, Quaternion root);

private:
	List<Model*> _models;

	void CreateDebugModels();
};
