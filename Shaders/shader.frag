#version 330 core
out vec4 FragColor;

uniform vec3 objectColor;
uniform vec3 lightColor;

void main()
{
    // For our physically based coloring we simply want to multiply the color of the light with the objects color
    // A much better and in depth explanation of this in the web tutorials.
    FragColor = vec4(lightColor * objectColor, 1.0);
}