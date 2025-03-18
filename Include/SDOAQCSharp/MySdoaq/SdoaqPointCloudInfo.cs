using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDOAQCSharp
{
    public class SdoaqPointCloudInfo : IDisposable
    {
        public readonly string Name;
        public readonly uint VertexDataSize;
		//public readonly float[] VertexDataBuffer;
		public float[] VertexDataBuffer { get; private set; }
		public readonly uint ImgDataSize;
		//public readonly byte[] ImgDataBuffer;
		public byte[] ImgDataBuffer { get; private set; }
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

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (ImgDataBuffer != null)
                    {
                        Array.Clear(ImgDataBuffer, 0, ImgDataBuffer.Length);
                    }

                    if (VertexDataBuffer != null)
                    {
                        Array.Clear(VertexDataBuffer, 0, VertexDataBuffer.Length);
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

		public void NullifyArray()
		{
			ImgDataBuffer = null;
			VertexDataBuffer = null;
		}
		#endregion
	}
}
