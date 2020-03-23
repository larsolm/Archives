#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace GrungySponge;

void UiBackground::Start()
{
	_totalDirt = 1280 * 720;
	_clearFieldManager->LoadLevel();

	DirtRenderer* dirtRenderer = CurrentGame()->World().GetObjects<DirtRenderer>().AllObjects().Last();
	dirtRenderer->SetTextures(_dirtManager->GetTexture("Dirt"), "", "");

	SpongeCamera* camera = CurrentGame()->World().GetObjects<SpongeCamera>().AllObjects().Last();
	camera->WorldBoundsBottom = -360;
	camera->WorldBoundsLeft = -640;
	camera->WorldBoundsRight = 640;
	camera->WorldBoundsTop = 360;
}

void UiBackground::Update(float elapsed)
{
	int clearedDirt = _clearFieldManager->ClearedSpace("Dirt", 0);

	if (clearedDirt / static_cast<float>(_totalDirt) > ClearPercentage / 100.0f)
		_clearFieldManager->Reset();
}

template<>
void Pargon::CreateMembers<UiBackground>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<UiBackground>();

	type->AddField("ClearPercentage", &UiBackground::ClearPercentage);
}
