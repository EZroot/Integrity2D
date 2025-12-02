using Integrity.Scenes;

namespace Integrity.Interface;
public interface ISceneManager : IService
{
    Scene? CurrentScene { get; }
    public void LoadScene(Scene scene);
    public void UnloadScene(Scene scene);
    public void AddScene(Scene scene);
    public Scene? GetScene(Guid id);
    public Scene? GetScene(string sceneName);
}