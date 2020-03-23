varying lowp vec2 textureOut;
varying mediump vec3 lightDirection;

uniform sampler2D sampler;

void main()
{
    mediump vec3 normal = texture2D(sampler, textureOut + vec2(0, 0.5)).xyz;
    normal = vec3(2.0) * (normal - vec3(0.5));
    
    mediump float diffuse = dot(normal, lightDirection) + 0.5;
    
    gl_FragColor = vec4(diffuse, diffuse, diffuse, 1) * texture2D(sampler, textureOut);
}
