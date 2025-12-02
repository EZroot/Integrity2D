using Integrity.Settings;

namespace Integrity.Interface;
public interface IEngineSettings : IService
{
    EngineSettings.EngineSettingsData Data { get; }
    Task LoadSettingsAsync();
    Task SaveSettingsAsync();
}