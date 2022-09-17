using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace Core;
internal class TextureArray : IDisposable {
    public int ID { get; private set; } = 0;

    public int Width { get; private set; } = 0;
    public int Height { get; private set; } = 0;
    public int TextureSlot { get; private set; } = 0;


    public TextureArray(int width, int height, int count) {
        Width = width;
        Height = height;

        ID = GL.GenTexture();
        Use();
        
        GL.TexStorage3D(
            TextureTarget3d.Texture2DArray, 1, SizedInternalFormat.Rgba8, width, height,
            count
        );

        GL.TexParameter(
            TextureTarget.Texture2DArray, 
            (TextureParameterName)OpenTK.Graphics.OpenGL.ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 
            16
        );

        GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
        GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
    }

    public void SetTexture(int index, TextureData data) {
        Use();
        GL.TexSubImage3D(
            TextureTarget.Texture2DArray, 0, 0, 0, index, data.Width, data.Height, 1,
            PixelFormat.Rgba, PixelType.UnsignedByte, data.DataPtr
        );
    }

    public void GenerateMipmaps() {
        Use();
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
    }

    public void Use() {
        GL.BindTexture(TextureTarget.Texture2DArray, ID);
    }

    public void Dispose() {
        Util.Logger.Success2($"Deleted TextureArray {ID} - {Width}x{Height} RGBA");

        GL.DeleteTexture(ID);
        ID = 0;
        Width = Height = 0;
    }
}