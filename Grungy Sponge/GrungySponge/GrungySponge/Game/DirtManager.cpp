#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace GrungySponge;

void DirtManager::LoadLevel(const LevelDescription& level, ClearFieldManager* clearFieldManager)
{
	DirtDescription* redDirt = GetDirt(level.RedDirt);
	DirtDescription* greenDirt = GetDirt(level.GreenDirt);
	DirtDescription* blueDirt = GetDirt(level.BlueDirt);

	DirtRenderer* dirtRenderer = CurrentGame()->World().GetObjects<DirtRenderer>().AllObjects().Last();
	dirtRenderer->SetTextures(redDirt == nullptr ? "" : redDirt->TextureFilename,
		greenDirt == nullptr ? "" : greenDirt->TextureFilename,
		blueDirt == nullptr ? "" : blueDirt->TextureFilename);

	_dirtData[0].Description = redDirt;
	_dirtData[1].Description = greenDirt;
	_dirtData[2].Description = blueDirt;

	float totalDirtFactor = 0.0f;
	for (int i = 0; i < 3; i++)
		totalDirtFactor += _dirtData[i].Description == nullptr ? 0.0f : _dirtData[i].Description->CleanContributionFactor;

	_totalDirt = 0;
	for (int i = 0; i < 3; i++)
	{
		_dirtData[i].ContributionPercent = _dirtData[i].Description == nullptr ? 0.0f : (_dirtData[i].Description->CleanContributionFactor / totalDirtFactor);
		_dirtData[i].ClearedDirt = 0;
		_dirtData[i].TotalDirt = clearFieldManager->TotalSpace("Dirt", i);
		_totalDirt += _dirtData[i].TotalDirt;
	}

	_levelDirtCapacity = static_cast<int>((level.DirtCapacity / 100.0f) * _totalDirt);
}

DirtDescription* DirtManager::GetDirt(const Pargon::String& dirt)
{
	if (dirt.IsEmpty())
		return nullptr;

	if (Dirts.ContainsKey(dirt))
		return Dirts[dirt].get();
    
	return nullptr;
}

const String& DirtManager::GetTexture(const String& dirt)
{
	if (Dirts.ContainsKey(dirt))
		return Dirts[dirt]->TextureFilename;
    else
        return "";
}

int DirtManager::CalculateDirtCapacity(const Array<int, 3>& heldDirt)
{
	int capacity = 0;

	for (int i = 0; i < 3; i++)
	{
		if (_dirtData[i].Description != nullptr)
			capacity += static_cast<int>(heldDirt[i] * _dirtData[i].Description->SpongeContributionFactor);
	}

	return capacity;
}

float DirtManager::CalculateClearedDirtPercent(ClearFieldManager* clearFieldManager)
{
	float percent = 0.0f;
	for (int i = 0; i < 3; i++)
	{
		if (_dirtData[i].TotalDirt == 0)
			continue;

		_dirtData[i].ClearedDirt = clearFieldManager->ClearedSpace("Dirt", i);
		float thisPercent = static_cast<float>(_dirtData[i].ClearedDirt) / static_cast<float>(_dirtData[i].TotalDirt);
		percent += thisPercent * _dirtData[i].ContributionPercent;
	}

	return percent * 100.0f;
}


template<>
void Pargon::CreateMembers<DirtManager>(Reflection::Type* type)
{
	type->AddField("Dirts", &DirtManager::Dirts);
}

template<>
void Pargon::CreateMembers<DirtDescription>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<DirtDescription>();

	type->AddField("Texture", &DirtDescription::TextureFilename);
	type->AddField("CleanContributionFactor", &DirtDescription::CleanContributionFactor);
	type->AddField("SpongeContributionFactor", &DirtDescription::SpongeContributionFactor);
}
