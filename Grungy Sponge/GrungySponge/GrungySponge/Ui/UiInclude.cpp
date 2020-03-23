#include "GrungySponge/GrungySponge.h"

using namespace Pargon;

void GrungySponge::Ui::Include()
{
    AddGameType<GameScreen, TouchMenu>("GrungySponge::GameScreen");
    AddGameType<UiBackground, Updater>("GrungySponge::UiBackground");
    AddGameType<IndexComponent, Component>("GrungySponge::IndexComponent");
    AddGameType<LevelList, TouchMenu>("GrungySponge::LevelList");
    AddGameType<ScrollContainer, Updater>("GrungySponge::ScrollContainer");
    AddGameType<Timer, Updater>("GrungySponge::Timer");
    AddGameType<WorldList, TouchMenu>("GrungySponge::WorldList");
}