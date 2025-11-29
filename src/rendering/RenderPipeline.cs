using System.Diagnostics;
using Silk.NET.OpenGL;
using Silk.NET.SDL;

public class RenderPipeline : IRenderPipeline
{
    public unsafe Renderer* m_RenderHandler;
    private readonly uint m_RendererFlags = (uint)RendererFlags.Accelerated;
    private Sdl? m_SdlApi;

    /// <summary>
    /// Initializes the renderer with the given SDL API and window.
    /// </summary>
    /// <param name="sdlApi"></param>
    /// <param name="window"></param>
    public unsafe void InitializeRenderer(Sdl sdlApi, Window* window)
    {
        m_SdlApi = sdlApi;
        m_RenderHandler = m_SdlApi.CreateRenderer(window, -1, m_RendererFlags);
    }
    
    public void RenderFrame()
    {
        RenderFrameStart();

        RenderFrameEnd();
    }

    private unsafe void RenderFrameStart()
    {
        Debug.Assert(m_SdlApi != null, "SDL API is not initialized in RenderPipeline.");
        m_SdlApi.SetRenderDrawColor(m_RenderHandler, 0, 0, 0, 255);
        m_SdlApi.RenderClear(m_RenderHandler);
    }

    private unsafe void RenderFrameEnd()
    {
        Debug.Assert(m_SdlApi != null, "SDL API is not initialized in RenderPipeline.");
        m_SdlApi.RenderPresent(m_RenderHandler);
    }
}