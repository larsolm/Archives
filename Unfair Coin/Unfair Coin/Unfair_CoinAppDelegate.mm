//
//  Unfair_CoinAppDelegate.m
//  Unfair Coin
//
//  Created by Adam Larson on 12/20/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "Unfair_CoinAppDelegate.h"

#import "EAGLView.h"

#import "Unfair_CoinViewController.h"

@implementation Unfair_CoinAppDelegate

@synthesize window = _window;
@synthesize viewController = _viewController;
@synthesize myAudioSessionManager;
@synthesize controller;
@synthesize observer;

- (AudioSessionManager *)myAudioSessionManager {
    if (myAudioSessionManager == nil) {
        myAudioSessionManager = [[AudioSessionManager alloc] init];
    }
    return myAudioSessionManager;
}

- (PocketsphinxController *)controller {
    if (controller == nil) {
        controller = [[PocketsphinxController alloc] init];
    }
    return controller;
}

- (OpenEarsEventsObserver *)observer {
    if (observer == nil) {
        observer = [[OpenEarsEventsObserver alloc] init];
    }
    return observer;
}

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    // Override point for customization after application launch.
    self.window.rootViewController = self.viewController;
    [self.myAudioSessionManager startAudioSession];
    
    NSString *lmPath = [NSString stringWithFormat:@"%@/%@",[[NSBundle mainBundle] resourcePath], @"unfair.languagemodel"];
    NSString *dictionaryPath = [NSString stringWithFormat:@"%@/%@",[[NSBundle mainBundle] resourcePath], @"unfair.dic"];
    [self.controller startListeningWithLanguageModelAtPath:lmPath dictionaryAtPath:dictionaryPath languageModelIsJSGF:NO];
    
    [self.observer setDelegate:self];
    
    return YES;
}

- (void)applicationWillResignActive:(UIApplication *)application
{
    /*
     Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
     Use this method to pause ongoing tasks, disable timers, and throttle down OpenGL ES frame rates. Games should use this method to pause the game.
     */
    [self.viewController stopAnimation];
}

- (void)applicationDidEnterBackground:(UIApplication *)application
{
    /*
     Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later. 
     If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.
     */
}

- (void)applicationWillEnterForeground:(UIApplication *)application
{
    /*
     Called as part of the transition from the background to the inactive state; here you can undo many of the changes made on entering the background.
     */
}

- (void)applicationDidBecomeActive:(UIApplication *)application
{
    /*
     Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
     */
    [self.viewController startAnimation];
}

- (void) pocketsphinxDidReceiveHypothesis:(NSString *)hypothesis recognitionScore:(NSString *)recognitionScore utteranceID:(NSString *)utteranceID
{
    NSLog(@"Pocketsphinx recognized the following phrase:%@", hypothesis);
}

- (void)applicationWillTerminate:(UIApplication *)application
{
    [self.controller stopListening];
    
    /*
     Called when the application is about to terminate.
     Save data if appropriate.
     See also applicationDidEnterBackground:.
     */
    [self.viewController stopAnimation];
}

- (void)dealloc
{
    [observer release];
    [controller release];
    [_window release];
    [_viewController release];
    [myAudioSessionManager release];
    [super dealloc];
}

@end
