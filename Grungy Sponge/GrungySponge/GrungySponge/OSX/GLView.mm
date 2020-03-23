#import "GLView.h"
#import "Pargon/GameCore/GameCore.h"
#import "Pargon/Graphics/OpenGL/GLDevice.h"
#import "Pargon/Audio/Apis/OpenAL11/ALDevice.h"
#import "OSXCanvas.h"

#import <OpenGL/gl.h>

@implementation GLView

-(void) awakeFromNib
{
    NSOpenGLPixelFormatAttribute attributes[] =
    {
        NSOpenGLPFADoubleBuffer,
        NSOpenGLPFADepthSize, 24,
        NSOpenGLPFAOpenGLProfile,
        NSOpenGLProfileVersion3_2Core,
        0
    };

    NSOpenGLPixelFormat* format = [[NSOpenGLPixelFormat alloc] initWithAttributes:attributes];
    NSOpenGLContext* context = [[NSOpenGLContext alloc] initWithFormat:format shareContext:nil];

	CGLEnable([context CGLContextObj], kCGLCECrashOnRemovedFunctions);
	
    [self setPixelFormat:format];
    [self setOpenGLContext:context];
}

-(void) prepareOpenGL
{
    [super prepareOpenGL];
    
    [[self openGLContext] makeCurrentContext];
    
    Pargon::CurrentGame()->AddGraphics<Pargon::Graphics::OpenGl43GraphicsDevice, Pargon::Graphics::OSXCanvas>(self);
    Pargon::CurrentGame()->AddAudio<Pargon::Audio::ALDevice>();
    
    GLint swapInterval = 1;
    [[self openGLContext] setValues:&swapInterval forParameter:NSOpenGLCPSwapInterval];
    
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(shutdown) name:NSApplicationWillTerminateNotification object:[NSApplication sharedApplication]];
    
    _timer = [NSTimer scheduledTimerWithTimeInterval:0.001 target:self selector:@selector(update) userInfo:nil repeats:YES];
}

-(void)update
{
    Pargon::CurrentGame()->Advance();
    
    [self setNeedsDisplay:YES];
}

- (void)drawRect:(NSRect)dirtyRect
{
	[[self openGLContext] makeCurrentContext];

    Pargon::CurrentGame()->Render();
    
    CGLFlushDrawable([[self openGLContext] CGLContextObj]);
}

- (void)mouseDown:(NSEvent *)theEvent
{
    //_game->Input()->SetMouseLeftButton(true);
}

- (void)mouseUp:(NSEvent *)theEvent
{
    //_game->Input()->SetMouseLeftButton(false);
}

- (void)rightMouseDown:(NSEvent *)theEvent
{
    //_game->Input()->SetMouseRightButton(true);
}

- (void)rightMouseUp:(NSEvent *)theEvent
{
    //_game->Input()->SetMouseRightButton(false);
}

- (void)mouseMoved:(NSEvent *)theEvent
{
    //NSPoint point = [self convertPoint:[theEvent locationInWindow] fromView:nil];
    //_game->Input()->SetMousePosition(point.x, point.y, self.frame.size.width, self.frame.size.height
}

- (void) shutdown
{
    
}

-(void) dealloc
{
    [[NSNotificationCenter defaultCenter] removeObserver:self];
    [self shutdown];
}

@end
