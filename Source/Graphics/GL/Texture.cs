using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace Core;
public class Texture : IDisposable {
    public int ID { get; private set; } = 0;

    public int Width { get; private set; } = 0;
    public int Height { get; private set; } = 0;
    public int TextureSlot { get; private set; } = 0;
    
    public Texture(string imagePath) {
        var data = new TextureData(imagePath);
        Width = data.Width;
        Height = data.Height;

        ID = GL.GenTexture();
        Use();

        GL.TexImage2D(
            TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, 
            PixelFormat.Rgba, PixelType.UnsignedByte, data.DataPtr
        );
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        
        data.Dispose();
        Util.Logger.Success2($"Loaded Texture {ID} - {Width}x{Height} RGBA ({imagePath})");
    }

    public void Use() {
        GL.BindTexture(TextureTarget.Texture2D, ID);
    }

    public void Dispose() {
        Util.Logger.Success2($"Deleted Texture {ID} - {Width}x{Height} RGBA");

        GL.DeleteTexture(ID);
        ID = 0;
        Width = Height = 0;
    }
}