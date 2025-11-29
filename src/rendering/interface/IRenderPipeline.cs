using Silk.NET.SDL;

public interface IRenderPipeline : IService
{
    unsafe void InitializeRenderer(Sdl sdlApi, Window* window);
    void RenderFrame();
}