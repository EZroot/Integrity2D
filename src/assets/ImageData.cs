public struct ImageData
{
    public byte[] PixelData;
    public int Width;
    public int Height;
    public int Channels;

    public ImageData(byte[] pixelData, int width, int height, int channels)
    {
        PixelData = pixelData;
        Width = width;
        Height = height;
        Channels = channels;
    }
}