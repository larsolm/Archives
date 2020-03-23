//
//  Unfair_CoinAppDelegate.h
//  Unfair Coin
//
//  Created by Adam Larson on 12/20/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "AudioSessionManager.h"
#import "PocketsphinxController.h"
#import "OpenEarsEventsObserver.h"

@class Unfair_CoinViewController;

@interface Unfair_CoinAppDelegate : NSObject <UIApplicationDelegate, OpenEarsEventsObserverDelegate>
{
    AudioSessionManager *myAudioSessionManager;
    PocketsphinxController *controller;
    OpenEarsEventsObserver *observer;
}

@property (nonatomic, retain) IBOutlet UIWindow *window;

@property (nonatomic, retain) IBOutlet Unfair_CoinViewController *viewController;

@property (nonatomic, retain) AudioSessionManager *myAudioSessionManager;

@property (nonatomic, retain) PocketsphinxController *controller;

@property (nonatomic, retain) OpenEarsEventsObserver *observer;

@end
