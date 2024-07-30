using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDOAQCSharp
{
    public class SdoaqImageInfo : ICloneable
    {
        public readonly string Name;
        public readonly int Width;
        public readonly int Height;
        public readonly int ColorByte;
        public readonly byte[] Data;

        public SdoaqImageInfo(string name, int width, int height, int colorByte, byte[] data)
        {
            Name = name;
            Width = width;
            Height = height;
            ColorByte = colorByte;
            Data = data;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
