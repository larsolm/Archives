attribute vec4 position;
attribute vec2 texture;
attribute vec3 normal;
attribute vec3 tangent;

varying vec2 textureOut;
varying vec3 lightDirection;

uniform mat4 transform;
uniform mat4 projection;

void main()
{
    vec3 binormal = normalize(cross(tangent, normal));
    lightDirection = mat3(tangent, binormal, normal) * normalize(vec3(1, 1, -1));
    
    gl_Position = projection * transform * position;
    textureOut = texture;
}
