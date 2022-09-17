#version 410 core

in vec3 TexCoords;
in float ColorIntensity;
out vec4 FragmentColor;

uniform sampler2DArray uTextures;

void main() {
    vec4 TexColor = texture(uTextures, TexCoords);
    FragmentColor = TexColor * ColorIntensity;
}