using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace Core;
internal class TextureData : IDisposable {
    public int Width { get; private set; }
    public int Height { get; private set; }
    public byte[] Data { get; private set; } 
    public IntPtr DataPtr { get; private set; }

    public TextureData(string loadPath, int width = 0, int height = 0) {
        var image = SixLabors.ImageSharp.Image.Load<Rgba32>(loadPath);

        if (image.Width < width && image.Height < height) {
            image.Mutate(x => x.Resize(width, height, new SixLabors.ImageSharp.Processing.Processors.Transforms.NearestNeighborResampler()));
        }

        var pixels = new List<byte>(4 * image.Width * image.Height);

        for (int y = 0; y < image.Height; y++) {
            for (int x = 0; x < image.Width; x++) {
                pixels.Add(image[x, y].R);
                pixels.Add(image[x, y].G);
                pixels.Add(image[x, y].B);
                pixels.Add(image[x, y].A);
            }
        }

        Width = image.Width;
        Height = image.Height;
        Data = pixels.ToArray();

        // Create pointer
        DataPtr = Marshal.AllocHGlobal(Data.Length);
        Marshal.Copy(Data, 0, DataPtr, Data.Length);
    }

    public void Dispose() {
        Marshal.FreeHGlobal(DataPtr);
        Data = new byte[0];
        Width = Height = 0;
    }
}