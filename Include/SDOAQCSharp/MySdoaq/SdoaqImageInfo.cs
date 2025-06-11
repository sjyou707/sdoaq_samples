using System;
using System.Collections.Generic;

namespace SDOAQCSharp
{
	public class SdoaqImageInfo : ICloneable, IDisposable
	{
		public readonly string Name;
		public readonly int Width;
		public readonly int Height;
		public readonly int ColorByte;
		//public readonly byte[] Data;
		public byte[] Data { get; private set; }

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

		#region IDisposable Support
		private bool _disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					if (Data != null)
					{
						Array.Clear(Data, 0, Data.Length);
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
			Data = null;
		}
        #endregion
    }

    public class MsgParaReceiveMf
    {
        public struct tRectInfo
        {
            public int Id;
            public int Step;
        }

        public int LastFilledRingBufferEntry;
        public IReadOnlyList<tRectInfo> RectInfoList = new List<tRectInfo>();
    };
}
