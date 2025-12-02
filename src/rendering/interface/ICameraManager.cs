using Integrity.Rendering;

namespace Integrity.Interface;
public interface ICameraManager : IService
{
    Camera2D? MainCamera { get; }
    void RegisterCamera(Camera2D camera);
}