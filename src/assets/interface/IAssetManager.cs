public interface IAssetManager : IService
{
    void InitializeAssets();
    ImageData LoadAsset(string assetPath);
}