#include "SuperMarioWar/SuperMarioWar.h"
#include "SuperMarioWar/CharacterSelectMenu.h"

using namespace SuperMarioWar;
using namespace Pargon;

void CharacterSelectMenu::Activate()
{
	auto directory = FileSystem::Directory("Assets/Images/Characters");
	auto contents = directory.GetContents();

	for (auto& file : contents.Files)
		_characters.Add(file.Filename());

	Menu::Activate();
}

void CharacterSelectMenu::Update()
{
	Menu::Update();
}

auto CharacterSelectMenu::IncrementSelection(int index, int amount) -> String
{
	auto selection = _characterIndexes[index];
	selection += amount;
	if (selection >= _characters.Count())
		selection = 0;
	else if (selection < 0)
		selection = _characters.Count() - 1;

	_characterIndexes[index] = selection;
	return _characters[selection];
}

template<>
void Pargon::CreateMembers<CharacterSelectMenu>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<CharacterSelectMenu>();

	type->AddMethod("IncrementSelection", &CharacterSelectMenu::IncrementSelection);
}
