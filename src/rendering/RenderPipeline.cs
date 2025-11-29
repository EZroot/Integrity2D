using System.Diagnostics;
using Silk.NET.OpenGL;
using Silk.NET.SDL;

public class RenderPipeline : IRenderPipeline
{
    private unsafe Renderer* m_RenderHandler;
    private unsafe Window* m_WindowHandler;
    public readonly System.Drawing.Color ClearColor = System.Drawing.Color.CornflowerBlue;
    private readonly uint m_RendererFlags = (uint)RendererFlags.Accelerated;
    private readonly ClearBufferMask m_ClearBufferMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit;
    private Sdl? m_SdlApi;
    private GL? m_GlApi;

    /// <summary>
    /// Initializes the renderer with the given SDL API and window.
    /// </summary>
    /// <param name="sdlApi"></param>
    /// <param name="window"></param>
    public unsafe void InitializeRenderer(Sdl sdlApi, Window* window)
    {
        m_SdlApi = sdlApi;
        m_WindowHandler = window;
        m_GlApi = GL.GetApi(GetProcAddress);
        Debug.Assert(m_GlApi != null, "Failed to initialize OpenGL API in RenderPipeline.");

        var settings = Service.Get<IEngineSettings>();
        Debug.Assert(settings != null, "Engine Settings service not found in RenderPipeline.");
        m_GlApi.Viewport(0,0, (uint)settings.Data.WindowWidth, (uint)settings.Data.WindowHeight);
    }

    public void RenderFrame()
    {
        RenderFrameStart();

        RenderFrameEnd();
    }

    private unsafe IntPtr GetProcAddress(string procName) 
    { 
        Debug.Assert(m_SdlApi != null, "SDL Api is null when getting proc address!");
        return (IntPtr)m_SdlApi.GLGetProcAddress(procName); 
    }

    private void RenderFrameStart()
    {
        Debug.Assert(m_GlApi != null, "SDL API is not initialized in RenderPipeline.");
        m_GlApi.ClearColor(ClearColor);
        m_GlApi.Clear(m_ClearBufferMask);
    }

    private unsafe void RenderFrameEnd()
    {
        Debug.Assert(m_SdlApi != null, "SDL API is not initialized in RenderPipeline.");
        m_SdlApi.GLSwapWindow(m_WindowHandler);
    }
}