public class CameraManager : ICameraManager
{
    private Camera2D? m_MainCamera;
    public Camera2D? MainCamera => m_MainCamera;

    public void RegisterCamera(Camera2D camera)
    {
        m_MainCamera = camera;
    }
}