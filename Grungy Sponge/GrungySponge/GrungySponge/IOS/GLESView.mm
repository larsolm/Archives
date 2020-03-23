#include "GrungySponge/GrungySponge.h"

#import "Pargon/Graphics/Apis/OpenGles30/OpenGles30GraphicsDevice.h"
#import "Pargon/Graphics/Apis/Ios/IosOpenGles30Canvas.h"
#import "Pargon/Audio/Apis/OpenAL11/ALDevice.h"

#import "GrungySponge/IOS/GLESView.h"

#import <CoreMotion/CoreMotion.h>

@interface GLESView ()
@property (strong, nonatomic) EAGLContext* context;
@property (nonatomic, strong) CMMotionManager* motionManager;
@end

@implementation GLESView

@synthesize context = _context;

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    self.context = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES3];
    GLKView* view = (GLKView*)self.view;
    
    view.context = self.context;
    [EAGLContext setCurrentContext:view.context];
    
    [self.view setMultipleTouchEnabled:YES];
    
    self.motionManager = [[CMMotionManager alloc] init];
    self.motionManager.deviceMotionUpdateInterval = 1.0 / 60.0;
    [self.motionManager startDeviceMotionUpdatesUsingReferenceFrame:CMAttitudeReferenceFrameXArbitraryCorrectedZVertical];

    Pargon::CurrentGame()->AddGraphics<Pargon::Graphics::OpenGles30GraphicsDevice, Pargon::Graphics::IosOpenGles30Canvas>(view);
    Pargon::CurrentGame()->AddAudio<Pargon::Audio::ALDevice>();
    Pargon::CurrentGame()->AddInput<Pargon::Input::KeyboardMouseDevice, Pargon::Input::ControllerDevice, Pargon::Input::TouchDevice, Pargon::Input::MotionDevice>();
    
    Pargon::CurrentGame()->LoadGame("GrungySponge.game");
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    
    if ([EAGLContext currentContext] == self.context)
        [EAGLContext setCurrentContext:nil];
    
    if(self.motionManager != nil)
    {
        [self.motionManager stopDeviceMotionUpdates];
        self.motionManager = nil;
    }
    
    self.context = nil;
}

- (void)glkView:(GLKView *)view drawInRect:(CGRect)rect
{    
    Pargon::CurrentGame()->Render();
}

- (void)update
{
    CMAttitude* attitude = self.motionManager.deviceMotion.attitude;
    Pargon::CurrentGame()->MotionInput()->SetMotionState(attitude.yaw, attitude.pitch, attitude.roll);
    Pargon::CurrentGame()->Advance();
}

- (void)touchesBegan:(NSSet*)touches withEvent:(UIEvent*)event
{
    Pargon::CurrentGame()->TouchInput()->SetTouch(true);
}

- (void)touchesMoved:(NSSet*)touches withEvent:(UIEvent*)event
{
    UITouch *touch = [touches anyObject];
    CGPoint position = [touch locationInView:self.view];
    
    Pargon::CurrentGame()->TouchInput()->SetTouchPosition(position.x, position.y, self.view.bounds.size.width, self.view.bounds.size.height);
}

- (void)touchesEnded:(NSSet*)touches withEvent:(UIEvent*)event
{
    Pargon::CurrentGame()->TouchInput()->SetTouch(false);
}

- (void)touchesCancelled:(NSSet*)touches withEvent:(UIEvent*)event
{
    [self touchesEnded:touches withEvent:event];
}

@end