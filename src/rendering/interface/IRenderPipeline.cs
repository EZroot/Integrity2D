using Silk.NET.OpenGL;
using Silk.NET.SDL;

public interface IRenderPipeline : IService
{
    GL? GlApi { get; }
    unsafe void InitializeRenderer(Sdl sdlApi, Window* window);
    void DrawTexture(GLTexture texture);
    void RenderFrameStart();
    void RenderFrameEnd();
}