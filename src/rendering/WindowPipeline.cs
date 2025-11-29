using System.Diagnostics;
using Silk.NET.OpenGL;
using Silk.NET.SDL;

public class WindowPipeline : IWindowPipeline
{
    private unsafe Window* m_WindowHandler;
    private Sdl? m_SdlApi;
    private IntPtr m_GlApi;

    private readonly WindowFlags m_WindowFlags = WindowFlags.Opengl | WindowFlags.Resizable | WindowFlags.AllowHighdpi;

    public unsafe Window* WindowHandler => m_WindowHandler;
    
    public unsafe void InitializeWindow(Sdl sdlApi, string title, int width, int height)
    {
        m_SdlApi = sdlApi;

        m_SdlApi.GLSetAttribute(GLattr.ContextMajorVersion, 3);
        m_SdlApi.GLSetAttribute(GLattr.ContextMinorVersion, 3);
        m_SdlApi.GLSetAttribute(GLattr.ContextProfileMask, (int)GLprofile.Core);
        m_SdlApi.GLSetAttribute(GLattr.Doublebuffer, 1);

        m_WindowHandler = m_SdlApi.CreateWindow(title, 0, 0, width, height, (uint)m_WindowFlags);
        Debug.Assert(m_WindowHandler != null, "Failed to create SDL Window.");
        Logger.Log("SDL Window created successfully.", Logger.LogSeverity.Info);

        m_SdlApi.SetHint(Sdl.HintRenderScaleQuality, "linear");

        m_GlApi = (IntPtr)m_SdlApi.GLCreateContext(m_WindowHandler);
        Debug.Assert(m_GlApi != IntPtr.Zero, "Failed to get OpenGL API from SDL.");

        m_SdlApi.GLMakeCurrent(m_WindowHandler, (void*)m_GlApi);
        m_SdlApi.GLSetSwapInterval(1); // Vsync
    }

    public bool ShouldClose()
    {
        return false;
    }
}