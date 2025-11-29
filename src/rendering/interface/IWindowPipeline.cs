using Silk.NET.SDL;

public interface IWindowPipeline : IService
{
    unsafe Window* WindowHandler { get; }
    void InitializeWindow(Sdl sdlApi, string title, int width, int height);
    bool ShouldClose();
}