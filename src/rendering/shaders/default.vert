#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoord;

// NEW UNIFORM: This will hold the scale, rotation, and position data
uniform mat4 model; 
// NEW UNIFORM: This is your camera/projection setup (e.g., converting pixel coords to NDC)
uniform mat4 projection; 

void main()
{
    // Apply the model matrix (scale, pos) and then the projection matrix (screen conversion)
    gl_Position = projection * model * vec4(aPos, 0.0, 1.0); 
    TexCoord = aTexCoord;
}