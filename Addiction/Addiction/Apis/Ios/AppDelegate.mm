#import "Addiction/Addiction.h"
#import "Addiction/Apis/Ios/AppDelegate.h"

#import <GLKit/GLKit.h>

#import "Pargon/GameCore/GameCore.h"
#import "Pargon/Graphics/Apis/OpenGles30/OpenGles30GraphicsDevice.h"
#import "Pargon/Graphics/Apis/Ios/IosOpenGles30Canvas.h"
#import "Pargon/Graphics/Apis/Ios/OpenGlesView.h"
#import "Pargon/Audio/Apis/OpenAl11/OpenAl11AudioDevice.h"
#import "Pargon/Input/Apis/Event/EventTouchDevice.h"

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

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    [OpenGlesView keepAtLinkTime];
    
    [[UIApplication sharedApplication] setStatusBarHidden:NO];
    
    UIViewController* rootController = self.window.rootViewController;
    GLKView* view = [rootController.view isKindOfClass:[GLKView class]] ? (GLKView*)rootController.view : nil;
    
    _gameWrapper._game->AddGraphics<Pargon::Graphics::OpenGles30GraphicsDevice, Pargon::Graphics::IosOpenGles30Canvas>(view);
    _gameWrapper._game->AddAudio<Pargon::Audio::OpenAl11AudioDevice>();
    _gameWrapper._game->AddTouchInput<Pargon::Input::EventTouchDevice>();
    
    _gameWrapper._game->LoadGame("Addiction.game");
    
    return YES;
}

- (void)applicationWillResignActive:(UIApplication *)application
{
    // Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
    // Use this method to pause ongoing tasks, disable timers, and throttle down OpenGL ES frame rates. Games should use this method to pause the game.
}

- (void)applicationDidEnterBackground:(UIApplication *)application
{
    // Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later.
    // If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.
}

- (void)applicationWillEnterForeground:(UIApplication *)application
{
    // Called as part of the transition from the background to the inactive state; here you can undo many of the changes made on entering the background.
}

- (void)applicationDidBecomeActive:(UIApplication *)application
{
    // Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
}

- (void)applicationWillTerminate:(UIApplication *)application
{
    _gameWrapper._game->UnloadLevel();
}

@end
