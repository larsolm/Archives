#import "Addiction/Addiction.h"
#import "Addiction/Apis/Osx/AppDelegate.h"

#import "Pargon/GameCore/GameCore.h"
#import "Pargon/Graphics/Apis/OpenGl43/OpenGl43GraphicsDevice.h"
#import "Pargon/Graphics/Apis/Osx/OsxCanvas.h"
#import "Pargon/Graphics/Apis/Osx/OpenGlView.h"
#import "Pargon/Audio/Apis/OpenAl11/OpenAl11AudioDevice.h"
#import "Pargon/Input/Apis/Osx/CocoaKeyboardMouseDevice.h"
#import "Pargon/Input/Apis/Emulated/MouseToTouchEmulator.h"


namespace
{
    struct GameWrapper
    {
        GameWrapper()
        {
            Addiction::Include();
            _game = std::make_unique<Pargon::Game>();
        }

        std::unique_ptr<Pargon::Game> _game;
    };

    GameWrapper _gameWrapper;
}

@implementation AppDelegate

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification
{
    NSWindow* window = [[[NSApplication sharedApplication] windows] firstObject];
    NSView* rootView = window.contentViewController.view;
    OpenGlView* view = [[rootView.subviews firstObject] isKindOfClass:[OpenGlView class]] ? (OpenGlView*)[rootView.subviews firstObject] : nil;
    
    _gameWrapper._game->AddGraphics<Pargon::Graphics::OpenGl43GraphicsDevice, Pargon::Graphics::OsxCanvas>(view);
    _gameWrapper._game->AddAudio<Pargon::Audio::OpenAl11AudioDevice>();
    _gameWrapper._game->AddTouchInput<Pargon::Input::MouseToTouchEmulator<Pargon::Input::CocoaKeyboardMouseDevice>>(view);
    
    _gameWrapper._game->LoadGame("Addiction.game");
}

- (void)applicationWillTerminate:(NSNotification *)aNotification
{
    _gameWrapper._game->UnloadLevel();
}

@end
