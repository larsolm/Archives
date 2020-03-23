varying lowp vec2 textureOut;
uniform sampler2D sampler;

void main()
{
    gl_FragColor = texture2D(sampler, textureOut);
}
