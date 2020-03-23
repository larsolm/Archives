#include "GrungySponge/GrungySponge.h"

using namespace Pargon;

void GrungySponge::Sponge::Include()
{
    AddGameType<SpongeData, Component>("GrungySponge::SpongeData");
    AddGameType<SpongeCamera, Camera2D>("GrungySponge::SpongeCamera");
    AddGameType<SpongeMover, Updater>("GrungySponge::SpongeMover");
    Reflection::AllTypes().Add<SpongeType>("GrungySponge::SpongeType");
}
