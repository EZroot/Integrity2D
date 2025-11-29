using System.Diagnostics;
using System.Runtime.CompilerServices;
using Silk.NET.SDL;

public class Engine
{
    private Sdl? m_SdlApi;
    private readonly EngineSettings m_Settings;
    private readonly IGame m_Game;
    private readonly IInputManager m_InputManager;
    private readonly IWindowPipeline m_WindowPipe;
    private readonly IRenderPipeline m_RenderPipe;
    private readonly ISceneManager m_SceneManager;
    private readonly IAudioManager m_AudioManager;

    private bool m_IsRunning;

    public Engine(IGame game)
    {
        m_Settings = new EngineSettings();

        m_InputManager = Service.Get<IInputManager>() ?? throw new Exception("Input Manager service not found.");
        m_WindowPipe = Service.Get<IWindowPipeline>() ?? throw new Exception("Window Pipeline service not found.");
        m_RenderPipe = Service.Get<IRenderPipeline>() ?? throw new Exception("Render Pipeline service not found.");
        m_SceneManager = Service.Get<ISceneManager>() ?? throw new Exception("Scene Manager service not found.");
        m_AudioManager = Service.Get<IAudioManager>() ?? throw new Exception("Audio Manager service not found.");

        m_Game = game;
    }

    public void Run()
    {
        Initialize();
        
        m_IsRunning = true;
        while(m_IsRunning)
        {
            HandleInput();
            Update();
            Render();
        }

        Cleanup();
    }

    public async Task InitializeAsync()
    {
        await m_Settings.LoadSettingsAsync();
        Logger.Log($"Engine '{m_Settings.EngineName}' version {m_Settings.EngineVersion} initialized.", Logger.LogSeverity.Info);
    }

    private unsafe void Initialize()
    {
        m_SdlApi = Sdl.GetApi();
        if (m_SdlApi.Init(Sdl.InitVideo) < 0)
        {
            throw new Exception("Failed to initialize SDL Video subsystem.");
        };
        Logger.Log("SDL Video subsystem initialized.", Logger.LogSeverity.Info);

        m_WindowPipe.InitializeWindow(m_SdlApi, m_Settings.WindowTitle, m_Settings.WindowWidth, m_Settings.WindowHeight);
        m_RenderPipe.InitializeRenderer(m_SdlApi, m_WindowPipe.WindowHandler);
        
        m_Game.Initialize();
    }

    private void HandleInput()
    {
        m_InputManager?.ProcessInput();
    }

    private void Update()
    {
        m_Game.Update();
    }

    private void Render()
    {
        m_RenderPipe.RenderFrame();
        
        m_Game.Render();
    }

    private void Cleanup()
    {
        m_Game.Cleanup();
    }
}