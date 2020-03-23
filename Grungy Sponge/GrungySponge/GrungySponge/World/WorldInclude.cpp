#include "GrungySponge/GrungySponge.h"

using namespace Pargon;

void GrungySponge::World::Include()
{
    AddGameType<ClearField, Component>("GrungySponge::ClearField");
    AddGameType<ClearFieldManager, GameObject>("GrungySponge::ClearFieldManager");
    AddGameType<DirtRenderer, Renderer>("GrungySponge::DirtRenderer");
    AddGameType<FinishLine, CollisionComponent>("GrungySponge::FinishLine");
    AddGameType<Pit, CollisionComponent>("GrungySponge::Pit");
    Reflection::AllTypes().Add<SpongeType>("GrungySponge::SpongeType");
}
