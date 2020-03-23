#ifndef Unfair_Coin_Model_h
#define Unfair_Coin_Model_h

#include <OpenGLES/ES2/gl.h>
#include <OpenGLES/ES2/glext.h>

class Model
{
public:
    void create();
    void destroy();
    void draw();
    
private:
    GLuint _vertexBuffer;
    GLuint _indexBuffer;
    
    int _indexCount;
};

#endif
