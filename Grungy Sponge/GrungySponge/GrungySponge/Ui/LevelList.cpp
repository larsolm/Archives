#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace Pargon::Text;
using namespace GrungySponge;

namespace
{
	struct LevelProcessor
	{
		void Load(const String& format, const Buffer& buffer, Blueprint& content)
		{
			Pon::Load(buffer, content);
		}
	};

	using LevelData = DataObject<Blueprint, LevelProcessor>;
}

void LevelList::Start()
{
	GameManager* gameManager = CurrentGame()->World().GetObjects<GameManager>().ObjectNamed("GameManager");

	LevelData data;
	data.SetSource<FileSource>("Worlds/" + gameManager->CurrentWorld.Name + "/world.data");

	*data >> _levelList;

	int levelIndex = 0;
	for (LevelDescription& level : _levelList)
	{
		Ensemble* levelEnsemble = static_cast<Ensemble*>(CurrentGame()->CreateObject(LevelTemplate));
		Spatial2D* spatial = levelEnsemble->GetComponent<Spatial2D>();

		IndexComponent* index = levelEnsemble->GetComponent<IndexComponent>();
		index->Index = levelIndex;

		spatial->Position.X = Left + ((spatial->Size.X + HorizontalSpacing) * (levelIndex % 5));
		spatial->Position.Y = Top - ((spatial->Size.Y + VerticalSpacing) * (levelIndex / 5));

		levelIndex++;

		TextObject* text = levelEnsemble->Children()[0]->GetComponent<TextObject>();
		text->Characters = ToString(levelIndex);
	}
}

void LevelList::SetLevel(unsigned int index)
{
	GameManager* gameManager = CurrentGame()->World().GetObjects<GameManager>().ObjectNamed("GameManager");
	gameManager->CurrentLevel = _levelList[index];
	gameManager->LevelIndex = index;
}

void LevelList::NextLevel()
{
	GameManager* gameManager = CurrentGame()->World().GetObjects<GameManager>().ObjectNamed("GameManager");
	UiStack* uiStack = CurrentGame()->World().GetObjects<UiStack>().ObjectNamed("UiStack");

	if (gameManager->LevelIndex + 1 >= _levelList.Count())
	{
		uiStack->Pop();
	}
	else
	{
		SetLevel(gameManager->LevelIndex + 1);
		uiStack->Push("GameScreen", false);
		uiStack->Push("LevelOverview", true);
	}
}

template<> void Pargon::CreateMembers<GrungySponge::LevelList>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<LevelList>();

	type->AddMethod("SetLevel", &LevelList::SetLevel);
	type->AddMethod("NextLevel", &LevelList::NextLevel);
	type->AddField("LevelTemplate", &LevelList::LevelTemplate);
	type->AddField("Top", &LevelList::Top);
	type->AddField("Left", &LevelList::Left);
	type->AddField("HorizontalSpacing", &LevelList::HorizontalSpacing);
	type->AddField("VerticalSpacing", &LevelList::VerticalSpacing);
}
