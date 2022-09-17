#version 410 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aTexCoords;
layout(location = 5) in float aColorIntensity;

out float ColorIntensity;
out vec3 TexCoords;

uniform mat4 uProjection;
uniform mat4 uModel;
uniform mat4 uView;

void main() {
    ColorIntensity = aColorIntensity;
    TexCoords = aTexCoords;

    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}