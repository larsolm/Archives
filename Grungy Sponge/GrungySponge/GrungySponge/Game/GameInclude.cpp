#include "GrungySponge/GrungySponge.h"

using namespace Pargon;

void GrungySponge::Game::Include()
{
    AddGameType<GameManager, GameObject>("GrungySponge::GameManager");
    AddGameType<DirtManager, GameObject>("GrungySponge::DirtManager");
    Reflection::AllTypes().Add<DirtDescription>("GrungySponge::DirtDescription");
    Reflection::AllTypes().Add<LevelDescription>("GrungySponge::LevelDescription");
    Reflection::AllTypes().Add<WorldDescription>("GrungySponge::WorldDescription");
}
