using System.Runtime.CompilerServices;

public class Engine
{
    private IntPtr m_WindowHandle;
    private IntPtr m_GlContext;
    private bool m_IsRunning = false;

    private readonly EngineSettings m_Settings;

    private readonly IGame m_Game;

    private readonly IInputManager? m_InputManager;
    private readonly IRenderer? m_Renderer;
    private readonly ISceneManager? m_SceneManager;
    private readonly IAudioManager? m_AudioManager;

    public Engine(IGame game)
    {
        m_Settings = new EngineSettings();

        m_InputManager = Service.Get<IInputManager>();
        m_Renderer = Service.Get<IRenderer>();
        m_SceneManager = Service.Get<ISceneManager>();
        m_AudioManager = Service.Get<IAudioManager>();

        m_Game = game;
    }

    public async Task InitializeAsync()
    {
        await m_Settings.LoadSettingsAsync();
        Logger.Log($"Engine '{m_Settings.EngineName}' version {m_Settings.EngineVersion} initialized.", Logger.LogSeverity.Info);
    }

    private void Initialize()
    {
        m_Game.Initialize();
    }

    public void Run()
    {
        Initialize();
        
        while(!m_IsRunning)
        {
            HandleInput();
            Update();
            Render();
        }

        Cleanup();
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
        m_Game.Render();
    }

    private void Cleanup()
    {
        m_Game.Cleanup();
    }
}