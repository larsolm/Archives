attribute vec4 position;
attribute vec2 texture;

varying vec2 textureOut;

void main()
{
    gl_Position = position;
    textureOut = texture;
}
