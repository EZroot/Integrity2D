using Silk.NET.SDL;

public class WindowPipeline : IWindowPipeline
{
    private unsafe Window* m_WindowHandler;
    private Sdl? m_SdlApi;

    public unsafe Window* WindowHandler => m_WindowHandler;
    
    public unsafe void InitializeWindow(Sdl sdlApi, string title, int width, int height)
    {
        m_SdlApi = sdlApi;
        m_WindowHandler = m_SdlApi.CreateWindow(title, 0, 0, width, height, (uint)WindowFlags.Opengl);
        if(m_WindowHandler == null)
        {
            throw new Exception("Failed to create SDL Window.");
        }
        Logger.Log("SDL Window created successfully.", Logger.LogSeverity.Info);

        m_SdlApi.SetWindowResizable(m_WindowHandler, SdlBool.True);
        m_SdlApi.SetHint(Sdl.HintRenderScaleQuality, "linear");
    }

    public bool ShouldClose()
    {
        return false;
    }
}