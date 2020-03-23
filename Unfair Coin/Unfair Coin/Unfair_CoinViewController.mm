//
//  Unfair_CoinViewController.m
//  Unfair Coin
//
//  Created by Adam Larson on 12/20/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <QuartzCore/QuartzCore.h>

#import "Unfair_CoinViewController.h"
#import "EAGLView.h"

// Uniform index.
enum {
    UNIFORM_TRANSLATE,
    UNIFORM_TRANSFORM,
    UNIFORM_PROJECTION,
    NUM_UNIFORMS
};
GLint uniforms[NUM_UNIFORMS];

// Attribute index.
enum {
    ATTRIB_VERTEX,
    ATTRIB_TEXTURE,
    ATTRIB_NORMAL,
    ATTRIB_TANGENT,
    NUM_ATTRIBUTES
};

@interface Unfair_CoinViewController ()
@property (nonatomic, retain) EAGLContext *context;
@property (nonatomic, assign) CADisplayLink *displayLink;
- (BOOL)loadShaders;
- (void)createModel;
- (BOOL)compileShader:(GLuint *)shader type:(GLenum)type file:(NSString *)file;
- (BOOL)linkProgram:(GLuint)prog;
- (BOOL)validateProgram:(GLuint)prog;
@end

@implementation Unfair_CoinViewController

@synthesize animating;
@synthesize context;
@synthesize displayLink;

- (void)awakeFromNib
{
    EAGLContext *aContext = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES2];
    
    if (!aContext) {
        aContext = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES1];
    }
    
    if (!aContext)
        NSLog(@"Failed to create ES context");
    else if (![EAGLContext setCurrentContext:aContext])
        NSLog(@"Failed to set ES context current");
    
	self.context = aContext;
	[aContext release];
	
    [(EAGLView *)self.view setContext:context];
    [(EAGLView *)self.view setFramebuffer];
    
    if ([context API] == kEAGLRenderingAPIOpenGLES2) {
        [self loadShaders];
        [self createModel];
    }
    
    animating = FALSE;
    animationFrameInterval = 1;
    self.displayLink = nil;
}

struct SimpleVertexData
{
    GLfloat position[4];
    GLfloat texture[2];
};

struct VertexData
{
    GLfloat position[4];
    GLfloat texture[2];
    GLfloat normal[3];
    GLfloat tangent[3];
};

- (void)createModel
{
    static float TWO_PI = 6.28;
    
    VertexData vertexData[82];
    unsigned char indexData[240];
    
    for (int i = 0; i < 20; i++)
    {
        indexData[3 * i] = i;
        indexData[3 * i + 1] = i + 1;
        indexData[3 * i + 2] = 80;
        
        indexData[3 * i + 60] = i + 20;
        indexData[3 * i + 62] = i + 21;
        indexData[3 * i + 61] = 81;
        
        indexData[3 * i + 120] = i + 40;
        indexData[3 * i + 122] = i + 41;
        indexData[3 * i + 121] = i + 60;
        
        indexData[3 * i + 180] = i + 60;
        indexData[3 * i + 181] = i + 61;
        indexData[3 * i + 182] = i + 41;
        
        if (i == 19)
        {
            indexData[3 * i + 1] = 0;
            indexData[3 * i + 62] = 20;
            
            indexData[3 * i + 122] = 40;
            
            indexData[3 * i + 181] = 60;
            indexData[3 * i + 182] = 40;
        }
        
        float angle = i * (TWO_PI / 20);
        float x = sinf(angle);
        float y = cosf(angle);
        
        vertexData[i].position[0] = x;
        vertexData[i].position[1] = y;
        vertexData[i].position[2] = 0.036;
        vertexData[i].position[3] = 1.0f;
        vertexData[i].texture[0] = x / 4 + 0.25f;
        vertexData[i].texture[1] = -y / 4 + 0.25f;
        vertexData[i].normal[0] = 0;
        vertexData[i].normal[1] = 0;
        vertexData[i].normal[2] = 1;
        vertexData[i].tangent[0] = y;
        vertexData[i].tangent[1] = -x;
        vertexData[i].tangent[2] = 0;
        
        vertexData[20 + i].position[0] = x;
        vertexData[20 + i].position[1] = y;
        vertexData[20 + i].position[2] = -0.036;
        vertexData[20 + i].position[3] = 1.0f;
        vertexData[20 + i].texture[0] = x / 4 + 0.75f;
        vertexData[20 + i].texture[1] = -y / 4 + 0.25f;
        vertexData[20 + i].normal[0] = -y;
        vertexData[20 + i].normal[1] = x;
        vertexData[20 + i].normal[2] = 0;
        vertexData[20 + i].tangent[0] = 0;
        vertexData[20 + i].tangent[1] = 0;
        vertexData[20 + i].tangent[2] = -1;
        
        vertexData[40 + i].position[0] = x;
        vertexData[40 + i].position[1] = y;
        vertexData[40 + i].position[2] = 0.036;
        vertexData[40 + i].position[3] = 1.0f;
        vertexData[40 + i].texture[0] = i;
        vertexData[40 + i].texture[1] = 0;
        vertexData[40 + i].normal[0] = x;
        vertexData[40 + i].normal[1] = y;
        vertexData[40 + i].normal[2] = 0;
        vertexData[40 + i].tangent[0] = 0;
        vertexData[40 + i].tangent[1] = 0;
        vertexData[40 + i].tangent[2] = 1;
        
        vertexData[60 + i].position[0] = x;
        vertexData[60 + i].position[1] = y;
        vertexData[60 + i].position[2] = -0.036;
        vertexData[60 + i].position[3] = 1.0f;
        vertexData[60 + i].texture[0] = i;
        vertexData[60 + i].texture[1] = 0.5;
        vertexData[60 + i].normal[0] = -x;
        vertexData[60 + i].normal[1] = -y;
        vertexData[60 + i].normal[2] = 0;
        vertexData[60 + i].tangent[0] = 0;
        vertexData[60 + i].tangent[1] = 0;
        vertexData[60 + i].tangent[2] = -1;
    }
    
    vertexData[80].position[0] = 0;
    vertexData[80].position[1] = 0;
    vertexData[80].position[2] = 0.036;
    vertexData[80].position[3] = 1.0f;
    vertexData[80].texture[0] = 0.25f;
    vertexData[80].texture[1] = 0.25f;
    vertexData[80].normal[0] = 0;
    vertexData[80].normal[1] = 0;
    vertexData[80].normal[2] = 1;
    vertexData[80].tangent[0] = 1;
    vertexData[80].tangent[1] = 0;
    vertexData[80].tangent[2] = 0;
    
    vertexData[81].position[0] = 0;
    vertexData[81].position[1] = 0;
    vertexData[81].position[2] = -0.036;
    vertexData[81].position[3] = 1.0f;
    vertexData[81].texture[0] = 0.75f;
    vertexData[81].texture[1] = 0.25f;
    vertexData[81].normal[0] = -1;
    vertexData[81].normal[1] = 0;
    vertexData[81].normal[2] = 0;
    vertexData[81].tangent[0] = 0;
    vertexData[81].tangent[1] = 0;
    vertexData[81].tangent[2] = -1;
    
    glGenBuffers(1, &vertexBuffer);
    GLenum error = glGetError();
    glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
    error = glGetError();
    glBufferData(GL_ARRAY_BUFFER, sizeof(VertexData) * 82, &vertexData, GL_STATIC_DRAW);
    error = glGetError();
    
    glGenBuffers(1, &indexBuffer);
    error = glGetError();
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffer);
    error = glGetError();
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, 240, &indexData, GL_STATIC_DRAW);
    error = glGetError();
    
    glGenTextures(1, &texture);
    glBindTexture(GL_TEXTURE_2D, texture);
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR); 
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    
    NSString *path = [[NSBundle mainBundle] pathForResource:@"quarter" ofType:@"png"];
    NSData *texData = [[NSData alloc] initWithContentsOfFile:path];
    UIImage *image = [[UIImage alloc] initWithData:texData];
    
    GLuint width = CGImageGetWidth(image.CGImage);
    GLuint height = CGImageGetHeight(image.CGImage);
    CGColorSpaceRef colorSpace = CGColorSpaceCreateDeviceRGB();
    void *imageData = malloc( height * width * 4 );
    CGContextRef cgContext = CGBitmapContextCreate( imageData, width, height, 8, 4 * width, colorSpace, kCGImageAlphaPremultipliedLast | kCGBitmapByteOrder32Big );
    CGColorSpaceRelease( colorSpace );
    CGContextClearRect( cgContext, CGRectMake( 0, 0, width, height ) );
    CGContextTranslateCTM( cgContext, 0, height - height );
    CGContextDrawImage( cgContext, CGRectMake( 0, 0, width, height ), image.CGImage );
    
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);
    glGenerateMipmap(GL_TEXTURE_2D);
    
    CGContextRelease(cgContext);
    
    free(imageData);
    [image release];
    [texData release];
    
    glGenTextures(1, &textureSide);
    glBindTexture(GL_TEXTURE_2D, textureSide);
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR); 
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    
    path = [[NSBundle mainBundle] pathForResource:@"quarterSide" ofType:@"png"];
    texData = [[NSData alloc] initWithContentsOfFile:path];
    image = [[UIImage alloc] initWithData:texData];
    
    width = CGImageGetWidth(image.CGImage);
    height = CGImageGetHeight(image.CGImage);
    colorSpace = CGColorSpaceCreateDeviceRGB();
    imageData = malloc( height * width * 4 );
    cgContext = CGBitmapContextCreate( imageData, width, height, 8, 4 * width, colorSpace, kCGImageAlphaPremultipliedLast | kCGBitmapByteOrder32Big );
    CGColorSpaceRelease( colorSpace );
    CGContextClearRect( cgContext, CGRectMake( 0, 0, width, height ) );
    CGContextTranslateCTM( cgContext, 0, height - height );
    CGContextDrawImage( cgContext, CGRectMake( 0, 0, width, height ), image.CGImage );
    
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);
    glGenerateMipmap(GL_TEXTURE_2D);
    
    CGContextRelease(cgContext);
    
    free(imageData);
    [image release];
    [texData release];
    
    SimpleVertexData backgroundVertices[4];
    unsigned char backgroundIndices[6] = { 0, 1, 2, 2, 0, 3 };
    
    backgroundVertices[0].position[0] = -1;
    backgroundVertices[0].position[1] = -1;
    backgroundVertices[0].position[2] = -1;
    backgroundVertices[0].position[3] = 1.0f;
    backgroundVertices[0].texture[0] = 0;
    backgroundVertices[0].texture[1] = 0;
    
    backgroundVertices[1].position[0] = -1;
    backgroundVertices[1].position[1] = 1;
    backgroundVertices[1].position[2] = -1;
    backgroundVertices[1].position[3] = 1.0f;
    backgroundVertices[1].texture[0] = 0;
    backgroundVertices[1].texture[1] = 1;
    
    backgroundVertices[2].position[0] = 1;
    backgroundVertices[2].position[1] = 1;
    backgroundVertices[2].position[2] = -1;
    backgroundVertices[2].position[3] = 1.0f;
    backgroundVertices[2].texture[0] = 1;
    backgroundVertices[2].texture[1] = 1;
    
    backgroundVertices[3].position[0] = 1;
    backgroundVertices[3].position[1] = -1;
    backgroundVertices[3].position[2] = -1;
    backgroundVertices[3].position[3] = 1.0f;
    backgroundVertices[3].texture[0] = 1;
    backgroundVertices[3].texture[1] = 0;
    
    glGenBuffers(1, &backgroundVB);
    glBindBuffer(GL_ARRAY_BUFFER, backgroundVB);
    glBufferData(GL_ARRAY_BUFFER, sizeof(SimpleVertexData) * 4, &backgroundVertices, GL_STATIC_DRAW);
    
    glGenBuffers(1, &backgroundIB);
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, backgroundIB);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, 6, &backgroundIndices, GL_STATIC_DRAW);
    
    glGenTextures(1, &backgroundTexture);
    glBindTexture(GL_TEXTURE_2D, backgroundTexture);
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MIN_FILTER, GL_LINEAR); 
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    
    path = [[NSBundle mainBundle] pathForResource:@"background" ofType:@"png"];
    texData = [[NSData alloc] initWithContentsOfFile:path];
    image = [[UIImage alloc] initWithData:texData];
    
    width = CGImageGetWidth(image.CGImage);
    height = CGImageGetHeight(image.CGImage);
    colorSpace = CGColorSpaceCreateDeviceRGB();
    imageData = malloc( height * width * 4 );
    cgContext = CGBitmapContextCreate( imageData, width, height, 8, 4 * width, colorSpace, kCGImageAlphaPremultipliedLast | kCGBitmapByteOrder32Big );
    CGColorSpaceRelease( colorSpace );
    CGContextClearRect( cgContext, CGRectMake( 0, 0, width, height ) );
    CGContextTranslateCTM( cgContext, 0, height - height );
    CGContextDrawImage( cgContext, CGRectMake( 0, 0, width, height ), image.CGImage );
    
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);
    
    CGContextRelease(cgContext);
    
    free(imageData);
    [image release];
    [texData release];
}

- (void)dealloc
{
    if (program) {
        glDeleteProgram(program);
        program = 0;
    }
    
    glDeleteTextures(1, &texture);
    glDeleteBuffers(1, &vertexBuffer);
    glDeleteBuffers(1, &indexBuffer);
    
    // Tear down context.
    if ([EAGLContext currentContext] == context)
        [EAGLContext setCurrentContext:nil];
    
    [context release];
    
    [super dealloc];
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc. that aren't in use.
}

- (void)viewWillAppear:(BOOL)animated
{
    [self startAnimation];
    
    [super viewWillAppear:animated];
}

- (void)viewWillDisappear:(BOOL)animated
{
    [self stopAnimation];
    
    [super viewWillDisappear:animated];
}

- (void)viewDidUnload
{
	[super viewDidUnload];
	
    if (program) {
        glDeleteProgram(program);
        program = 0;
    }

    // Tear down context.
    if ([EAGLContext currentContext] == context)
        [EAGLContext setCurrentContext:nil];
	self.context = nil;	
}

- (NSInteger)animationFrameInterval
{
    return animationFrameInterval;
}

- (void)setAnimationFrameInterval:(NSInteger)frameInterval
{
    /*
	 Frame interval defines how many display frames must pass between each time the display link fires.
	 The display link will only fire 30 times a second when the frame internal is two on a display that refreshes 60 times a second. The default frame interval setting of one will fire 60 times a second when the display refreshes at 60 times a second. A frame interval setting of less than one results in undefined behavior.
	 */
    if (frameInterval >= 1) {
        animationFrameInterval = frameInterval;
        
        if (animating) {
            [self stopAnimation];
            [self startAnimation];
        }
    }
}

- (void)startAnimation
{
    if (!animating) {
        CADisplayLink *aDisplayLink = [[UIScreen mainScreen] displayLinkWithTarget:self selector:@selector(drawFrame)];
        [aDisplayLink setFrameInterval:animationFrameInterval];
        [aDisplayLink addToRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
        self.displayLink = aDisplayLink;
        
        xRotation = 0.0;
        zPosition = 0.0;
        zDirection = 0.1;
        
        animating = TRUE;
    }
}

- (void)stopAnimation
{
    if (animating) {
        [self.displayLink invalidate];
        self.displayLink = nil;
        animating = FALSE;
    }
}

- (void)drawFrame
{
    [(EAGLView *)self.view setFramebuffer];
    
    glClearColor(0.5f, 0.5f, 0.5f, 1.0f);
    glClearDepthf(1.0f);
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    
    
    glDepthMask(false);
    glDisable(GL_DEPTH_TEST);
    glDisable(GL_CULL_FACE);
    
    glUseProgram(bgProgram);
    
    glBindBuffer(GL_ARRAY_BUFFER, backgroundVB);
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, backgroundIB);
    
    glVertexAttribPointer(ATTRIB_VERTEX, 4, GL_FLOAT, 0, sizeof(SimpleVertexData), 0);
    glEnableVertexAttribArray(ATTRIB_VERTEX);
    glVertexAttribPointer(ATTRIB_TEXTURE, 2, GL_FLOAT, 1, sizeof(SimpleVertexData), (char*)16);
    glEnableVertexAttribArray(ATTRIB_TEXTURE);
    
    glBindTexture(GL_TEXTURE_2D, backgroundTexture);
    glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_BYTE, 0);
    
    glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffer);
    
    
    if ([context API] == kEAGLRenderingAPIOpenGLES2) {
        // Use shader program.
        glUseProgram(program);
        
        // Update attribute values.
        glVertexAttribPointer(ATTRIB_VERTEX, 4, GL_FLOAT, 0, sizeof(VertexData), 0);
        glEnableVertexAttribArray(ATTRIB_VERTEX);
        glVertexAttribPointer(ATTRIB_TEXTURE, 2, GL_FLOAT, 1, sizeof(VertexData), (char*)16);
        glEnableVertexAttribArray(ATTRIB_TEXTURE);
        glVertexAttribPointer(ATTRIB_NORMAL, 3, GL_FLOAT, 0, sizeof(VertexData), (char*)24);
        glEnableVertexAttribArray(ATTRIB_NORMAL);
        glVertexAttribPointer(ATTRIB_TANGENT, 3, GL_FLOAT, 0, sizeof(VertexData), (char*)36);
        glEnableVertexAttribArray(ATTRIB_TANGENT);
        
        // Validate program before drawing. This is a good check, but only really necessary in a debug build.
        // DEBUG macro must be defined in your debug configurations if that's not already the case.
#if defined(DEBUG)
        if (![self validateProgram:program]) {
            NSLog(@"Failed to validate program: %d", program);
            return;
        }
#endif
    }
    
    xRotation -= 0.2;
    
    float rsin = sinf(xRotation);
    float rcos = cosf(xRotation);
    
    GLfloat transform[16] = {
        1,0,0,0,
        0,rcos,rsin,0,
        0,-rsin,rcos,0,
        0,0,-4,1
    };
    
    float near = 0.001, far = 100.0;
    float angleOfView = 0.785;
    float aspectRatio = 0.66f;
    
    float size = near * tanf(angleOfView / 2.0);
    float left = -size, right = size, bottom = -size / aspectRatio, top = size / aspectRatio;
    
    float w = 2 * near / (right - left);
    float h = 2 * near / (top - bottom);
    
    float d1 = (right + left) / (right - left);
    float d2 = (top + bottom) / (top - bottom);
    float d3 = -(far + near) / (far - near);
    float d4 = -(2 * far * near) / (far - near);
    
    GLfloat projection[16] = {
        w, 0, 0, 0,
        0, h, 0, 0,
        d1, d2, d3, -1,
        0, 0, d4, 0
    };
    
    glDepthMask(true);
    glEnable(GL_DEPTH_TEST);
    glEnable(GL_CULL_FACE);
    glCullFace(GL_FRONT);
    
    glUniformMatrix4fv(uniforms[UNIFORM_TRANSFORM], 1, GL_FALSE, transform);
    glUniformMatrix4fv(uniforms[UNIFORM_PROJECTION], 1, GL_FALSE, projection);
    
    glBindTexture(GL_TEXTURE_2D, texture);
    glDrawElements(GL_TRIANGLES, 120, GL_UNSIGNED_BYTE, 0);
    
    glBindTexture(GL_TEXTURE_2D, textureSide);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_MIRRORED_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_MIRRORED_REPEAT);
    glDrawElements(GL_TRIANGLES, 120, GL_UNSIGNED_BYTE, (char*)120);
    
    [(EAGLView *)self.view presentFramebuffer];
}

- (BOOL)compileShader:(GLuint *)shader type:(GLenum)type file:(NSString *)file
{
    GLint status;
    const GLchar *source;
    
    source = (GLchar *)[[NSString stringWithContentsOfFile:file encoding:NSUTF8StringEncoding error:nil] UTF8String];
    if (!source)
    {
        NSLog(@"Failed to load vertex shader");
        return FALSE;
    }
    
    *shader = glCreateShader(type);
    glShaderSource(*shader, 1, &source, NULL);
    glCompileShader(*shader);
    
#if defined(DEBUG)
    GLint logLength;
    glGetShaderiv(*shader, GL_INFO_LOG_LENGTH, &logLength);
    if (logLength > 0)
    {
        GLchar *log = (GLchar *)malloc(logLength);
        glGetShaderInfoLog(*shader, logLength, &logLength, log);
        NSLog(@"Shader compile log:\n%s", log);
        free(log);
    }
#endif
    
    glGetShaderiv(*shader, GL_COMPILE_STATUS, &status);
    if (status == 0)
    {
        glDeleteShader(*shader);
        return FALSE;
    }
    
    return TRUE;
}

- (BOOL)linkProgram:(GLuint)prog
{
    GLint status;
    
    glLinkProgram(prog);
    
#if defined(DEBUG)
    GLint logLength;
    glGetProgramiv(prog, GL_INFO_LOG_LENGTH, &logLength);
    if (logLength > 0)
    {
        GLchar *log = (GLchar *)malloc(logLength);
        glGetProgramInfoLog(prog, logLength, &logLength, log);
        NSLog(@"Program link log:\n%s", log);
        free(log);
    }
#endif
    
    glGetProgramiv(prog, GL_LINK_STATUS, &status);
    if (status == 0)
        return FALSE;
    
    return TRUE;
}

- (BOOL)validateProgram:(GLuint)prog
{
    GLint logLength, status;
    
    glValidateProgram(prog);
    glGetProgramiv(prog, GL_INFO_LOG_LENGTH, &logLength);
    if (logLength > 0)
    {
        GLchar *log = (GLchar *)malloc(logLength);
        glGetProgramInfoLog(prog, logLength, &logLength, log);
        NSLog(@"Program validate log:\n%s", log);
        free(log);
    }
    
    glGetProgramiv(prog, GL_VALIDATE_STATUS, &status);
    if (status == 0)
        return FALSE;
    
    return TRUE;
}

- (BOOL)loadShaders
{
    GLuint vertShader, fragShader;
    NSString *vertShaderPathname, *fragShaderPathname;
    
    // Create shader program.
    program = glCreateProgram();
    
    // Create and compile vertex shader.
    vertShaderPathname = [[NSBundle mainBundle] pathForResource:@"Shader" ofType:@"vsh"];
    if (![self compileShader:&vertShader type:GL_VERTEX_SHADER file:vertShaderPathname])
    {
        NSLog(@"Failed to compile vertex shader");
        return FALSE;
    }
    
    // Create and compile fragment shader.
    fragShaderPathname = [[NSBundle mainBundle] pathForResource:@"Shader" ofType:@"fsh"];
    if (![self compileShader:&fragShader type:GL_FRAGMENT_SHADER file:fragShaderPathname])
    {
        NSLog(@"Failed to compile fragment shader");
        return FALSE;
    }
    
    // Attach vertex shader to program.
    glAttachShader(program, vertShader);
    
    // Attach fragment shader to program.
    glAttachShader(program, fragShader);
    
    // Bind attribute locations.
    // This needs to be done prior to linking.
    glBindAttribLocation(program, ATTRIB_VERTEX, "position");
    glBindAttribLocation(program, ATTRIB_TEXTURE, "texture");
    glBindAttribLocation(program, ATTRIB_NORMAL, "tangent");
    glBindAttribLocation(program, ATTRIB_TANGENT, "normal");
    
    // Link program.
    if (![self linkProgram:program])
    {
        NSLog(@"Failed to link program: %d", program);
        
        if (vertShader)
        {
            glDeleteShader(vertShader);
            vertShader = 0;
        }
        if (fragShader)
        {
            glDeleteShader(fragShader);
            fragShader = 0;
        }
        if (program)
        {
            glDeleteProgram(program);
            program = 0;
        }
        
        return FALSE;
    }
    
    uniforms[UNIFORM_TRANSLATE] = glGetUniformLocation(program, "sampler");
    uniforms[UNIFORM_TRANSFORM] = glGetUniformLocation(program, "transform");
    uniforms[UNIFORM_PROJECTION] = glGetUniformLocation(program, "projection");
    
    // Release vertex and fragment shaders.
    if (vertShader)
        glDeleteShader(vertShader);
    if (fragShader)
        glDeleteShader(fragShader);
    
    
    // Create shader program.
    bgProgram = glCreateProgram();
    
    // Create and compile vertex shader.
    vertShaderPathname = [[NSBundle mainBundle] pathForResource:@"ShaderBG" ofType:@"vsh"];
    if (![self compileShader:&vertShader type:GL_VERTEX_SHADER file:vertShaderPathname])
    {
        NSLog(@"Failed to compile vertex shader");
        return FALSE;
    }
    
    // Create and compile fragment shader.
    fragShaderPathname = [[NSBundle mainBundle] pathForResource:@"ShaderBG" ofType:@"fsh"];
    if (![self compileShader:&fragShader type:GL_FRAGMENT_SHADER file:fragShaderPathname])
    {
        NSLog(@"Failed to compile fragment shader");
        return FALSE;
    }
    
    // Attach vertex shader to program.
    glAttachShader(bgProgram, vertShader);
    
    // Attach fragment shader to program.
    glAttachShader(bgProgram, fragShader);
    
    // Bind attribute locations.
    // This needs to be done prior to linking.
    glBindAttribLocation(bgProgram, ATTRIB_VERTEX, "position");
    glBindAttribLocation(bgProgram, ATTRIB_TEXTURE, "texture");
    
    // Link program.
    if (![self linkProgram:bgProgram])
    {
        NSLog(@"Failed to link program: %d", bgProgram);
        
        if (vertShader)
        {
            glDeleteShader(vertShader);
            vertShader = 0;
        }
        if (fragShader)
        {
            glDeleteShader(fragShader);
            fragShader = 0;
        }
        if (bgProgram)
        {
            glDeleteProgram(bgProgram);
            bgProgram = 0;
        }
        
        return FALSE;
    }
    
    // Release vertex and fragment shaders.
    if (vertShader)
        glDeleteShader(vertShader);
    if (fragShader)
        glDeleteShader(fragShader);
    
    return TRUE;
}

@end
