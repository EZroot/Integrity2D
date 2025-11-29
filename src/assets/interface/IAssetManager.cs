public interface IAssetManager : IService
{
    void InitializeAssets();
    void LoadAsset(string assetPath);
}