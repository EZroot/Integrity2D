public interface IAudioManager : IService
{
    void InitializeAudio();
    void PlaySound(string soundPath);
}