using Silk.NET.SDL;

namespace Integrity.Interface;
public interface IInputManager : IService
{
    unsafe void ProcessInput(Event ev);
    bool IsKeyDown(Scancode scancode);
}