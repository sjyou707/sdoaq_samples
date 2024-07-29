namespace SDOAQCSharp.Tool
{
    public class SdoaqImageInfo
    {
        public readonly int Width;
        public readonly int Height;
        public readonly int PixelBytes;
        public readonly int Line;
        public readonly byte[] Data;

        public SdoaqImageInfo(int width, int height, int line, int pixelByte, byte[] data)
        {
            Width = width;
            Height = height;
            Line = line;
            PixelBytes = pixelByte;
            Data = data;
        }
    }
}
