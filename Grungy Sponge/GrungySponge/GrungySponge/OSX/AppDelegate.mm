#include "GrungySponge/GrungySponge.h"

#import "AppDelegate.h"

namespace
{
    struct GameWrapper
    {
        GameWrapper()
        {
            GrungySponge::Include();
            _game = std::make_unique<Pargon::Game>();
        }

        std::unique_ptr<Pargon::Game> _game;
    };

    GameWrapper _gameWrapper;
}

@implementation AppDelegate

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification
{
    _gameWrapper._game->LoadGame("GrungySponge.game");
}

- (void)applicationWillTerminate:(NSNotification *)aNotification
{
    _gameWrapper._game->UnloadLevel();
}

@end
