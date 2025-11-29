public class EngineSettings
{
    public const string FILENAME_ENGINE_SETTINGS = "engine.ini";

    public string EngineName { get; set; } = "Integrity";
    public string EngineVersion { get; set; } = "0.1.0";
    public int WindowWidth { get; set; } = 1280;
    public int WindowHeight { get; set; } = 720;
    public string WindowTitle { get; set; } = "Integrity Engine";
    
    public async Task LoadSettingsAsync()
    {
        string path = Path.Combine(AppContext.BaseDirectory, FILENAME_ENGINE_SETTINGS);
        try
        {
            string content = await File.ReadAllTextAsync(path);
            EngineName = ParseSetting(content, "EngineName", EngineName);
            EngineVersion = ParseSetting(content, "EngineVersion", EngineVersion);
            WindowWidth = int.Parse(ParseSetting(content, "WindowWidth", WindowWidth.ToString()));
            WindowHeight = int.Parse(ParseSetting(content, "WindowHeight", WindowHeight.ToString()));
            WindowTitle = ParseSetting(content, "WindowTitle", WindowTitle);

            if (WindowWidth <= 0) WindowWidth = 1280;
            if (WindowHeight <= 0) WindowHeight = 720;
        }
        catch (FileNotFoundException)
        {
            Logger.Log($"Settings file not found at {path}. Recreating the engine settings.", Logger.LogSeverity.Warning);
            await SaveSettingsAsync();
        }
        catch (Exception ex)
        {
            Logger.Log($"Error loading settings: {ex.Message}", Logger.LogSeverity.Error);
        }
    }

    public async Task SaveSettingsAsync()
    {
        string path = Path.Combine(AppContext.BaseDirectory, FILENAME_ENGINE_SETTINGS);
        try
        {
            using (var writer = new StreamWriter(path, false))
            {
                await writer.WriteLineAsync($"[Engine]");
                await writer.WriteLineAsync($"EngineName={EngineName}");
                await writer.WriteLineAsync($"EngineVersion={EngineVersion}");
                await writer.WriteLineAsync($"[Window]");
                await writer.WriteLineAsync($"WindowWidth={WindowWidth}");
                await writer.WriteLineAsync($"WindowHeight={WindowHeight}");
                await writer.WriteLineAsync($"WindowTitle={WindowTitle}");
            }

            Logger.Log($"Settings saved to {path}.", Logger.LogSeverity.Info);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error Saving settings: {ex.Message}", Logger.LogSeverity.Error);
        }
    }

    private string ParseSetting(string content, string key, string defaultValue)
    {
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            if (line.StartsWith(";") || line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
            {
                return line.Substring(key.Length + 1).Trim();
            }
        }
        return defaultValue;
    }
}