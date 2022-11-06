#version 330

layout(location = 0) out vec3 color;
out vec4 outputColor;
in vec2 texCoord;

uniform sampler2D texture0;
uniform sampler2D texture1;
uniform vec4 overlayColor;

void main()
{
    outputColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.7) * overlayColor;

    if(outputColor.a <= 0)
        discard;
    // outputColor = texture(texture0, texCoord);
}