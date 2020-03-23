#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace Pargon::Text;
using namespace GrungySponge;

namespace
{
	struct WorldProcessor
	{
		void Load(const String& format, const Buffer& buffer, Blueprint& content)
		{
			Pon::Load(buffer, content);
		}
	};

	using WorldData = DataObject<Blueprint, WorldProcessor>;
}

void WorldList::Start()
{
	WorldData data;
	data.SetSource<FileSource>("Worlds/worlds.data");

	*data >> _worldList;

	int worldIndex = 0;
	for (WorldDescription& world : _worldList)
	{
        Ensemble* worldEnsemble = static_cast<Ensemble*>(CurrentGame()->CreateObject(WorldTemplate));
		Spatial2D* spatial = worldEnsemble->GetComponent<Spatial2D>();

		IndexComponent* index = worldEnsemble->GetComponent<IndexComponent>();
		index->Index = worldIndex;

		TextObject* text = worldEnsemble->Children()[0]->GetComponent<TextObject>();
		text->Characters = world.Name;

		_createdObjects.Add(worldEnsemble);
		worldIndex++;
	}
}

void WorldList::SetWorld(unsigned int index)
{
	GameManager* gameManager = CurrentGame()->World().GetObjects<GameManager>().ObjectNamed("GameManager");
	gameManager->CurrentWorld = _worldList[index];
}

template<> void Pargon::CreateMembers<GrungySponge::WorldList>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<WorldList>();

	type->AddMethod("SetWorld", &WorldList::SetWorld);
	type->AddField("WorldTemplate", &WorldList::WorldTemplate);
	type->AddField("Position", &WorldList::Position);
	type->AddField("Spacing", &WorldList::Spacing);
}
