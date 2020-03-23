//
//  Unfair_CoinViewController.h
//  Unfair Coin
//
//  Created by Adam Larson on 12/20/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>

#import <OpenGLES/EAGL.h>

#import <OpenGLES/ES1/gl.h>
#import <OpenGLES/ES1/glext.h>
#import <OpenGLES/ES2/gl.h>
#import <OpenGLES/ES2/glext.h>

@interface Unfair_CoinViewController : UIViewController {
    EAGLContext *context;
    GLuint program;
    
    GLuint bgProgram;
    
    BOOL animating;
    NSInteger animationFrameInterval;
    CADisplayLink *displayLink;
    
    GLuint vertexBuffer;
    GLuint indexBuffer;
    GLuint texture;
    GLuint textureSide;
    
    float xRotation;
    float zPosition;
    float zDirection;
    
    GLuint backgroundVB;
    GLuint backgroundIB;
    GLuint backgroundTexture;
}

@property (readonly, nonatomic, getter=isAnimating) BOOL animating;
@property (nonatomic) NSInteger animationFrameInterval;

- (void)startAnimation;
- (void)stopAnimation;

@end
