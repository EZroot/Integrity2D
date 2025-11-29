using Silk.NET.OpenGL;
using System.Runtime.InteropServices; // Needed for Marshal

public class GLTexture
{
    public uint TextureId { get; private set; }
    public int Width { get; }
    public int Height { get; }
    
    // Store the GL API instance
    private readonly GL m_GlApi;

    public GLTexture(GL glApi, ImageData imageData)
    {
        m_GlApi = glApi;
        Width = imageData.Width;
        Height = imageData.Height;
        
        TextureId = CreateTexture(imageData);
    }

    private unsafe uint CreateTexture(ImageData imageData)
    {
        uint textureId = m_GlApi.GenTexture();
        
        m_GlApi.BindTexture(TextureTarget.Texture2D, textureId);

        // --- Set Texture Parameters ---
        // Basic Linear filtering and Clamp-to-Edge wrapping
        m_GlApi.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
        m_GlApi.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
        m_GlApi.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        m_GlApi.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

        // --- Load Data into GPU ---
        fixed (byte* ptr = imageData.PixelData)
        {
            m_GlApi.TexImage2D(
                TextureTarget.Texture2D,
                0, // mipmap level
                (int)InternalFormat.Rgba, // internal format
                (uint)imageData.Width,
                (uint)imageData.Height,
                0, // border
                PixelFormat.Rgba, // source format (matches StbImageSharp output)
                PixelType.UnsignedByte, // source data type
                ptr
            );
        }

        m_GlApi.GenerateMipmap(TextureTarget.Texture2D);
        
        m_GlApi.BindTexture(TextureTarget.Texture2D, 0); // Unbind
        
        return textureId;
    }
    
    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        m_GlApi.ActiveTexture(unit);
        m_GlApi.BindTexture(TextureTarget.Texture2D, TextureId);
    }
    
    // Clean up GPU resource
    public void Dispose()
    {
        m_GlApi.DeleteTexture(TextureId);
    }
}