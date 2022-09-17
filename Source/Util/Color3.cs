namespace Util;
struct Color3 {
    public byte R;
    public byte G;
    public byte B;

    public Color3() {
        R = G = B = 1;
    }
    
    public Color3(byte r, byte g, byte b) {
        R = r;
        G = g;
        B = b;
    }

    public Color3(byte color) {
        R = G = B = color;
    }

    public byte[] ToArray => new byte[3] { R, G, B };

    public static float SizeInBytes => sizeof(byte) * 3;
}