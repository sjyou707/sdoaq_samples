using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDOAQCSharp
{
    public class SdoaqPointCloudInfo
    {
        public readonly string Name;
        public readonly uint VertexDataSize;
        public readonly float[] VertexDataBuffer;
        public readonly uint ImgDataSize;
        public readonly byte[] ImgDataBuffer;
        public readonly int Width;
        public readonly int Height;
        public readonly int SliceCount;
        public SdoaqPointCloudInfo(string name, 
            int width, int height, int sliceCount,
            float[] vertexDataBuffer, uint vertexDataSize, 
            byte[] imgDataBuffer, uint imgDataSize)
        {
            Name = name;
            Width = width;
            Height = height;
            SliceCount = sliceCount;
            VertexDataSize = vertexDataSize;
            VertexDataBuffer = vertexDataBuffer;
            ImgDataSize = imgDataSize;
            ImgDataBuffer = imgDataBuffer;
        }
    }
}
