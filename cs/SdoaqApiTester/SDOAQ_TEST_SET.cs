using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static SDOAQ.SDOAQ_API;

namespace SDOAQ_App_CS
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	unsafe public struct tRingBuf
	{
		public bool active;
		public IntPtr[] ppBuf;
		public ulong[] pSizes;
		public uint numsBuf;
		public byte* pointerToFirst;
	};

	public class cFocus
	{
		public cFocus()
		{
			numsFocus = 10;
		}
		public uint numsFocus;
		public List<int> vFocusSet = new List<int>();
	}

	public class SDOAQ_TEST_SET
	{
		public SDOAQ_TEST_SET()
		{
			afp.cameraRoiTop = 0;
			afp.cameraRoiLeft = 0;
			afp.cameraRoiWidth = (2064 / 4) * 4;
			afp.cameraRoiHeight = 1544;
			afp.cameraBinning = 1;
		}
		public void ClearBuffer()
		{
            buffer = null;

            if (rb.ppBuf != null)
			{
				for (int idx = 0; idx < rb.ppBuf.Length; idx++)
				{
					if (rb.ppBuf[idx] != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(rb.ppBuf[idx]);
						rb.ppBuf[idx] = IntPtr.Zero;
					}
				}
			}
		}
		public void AllocBuffer(int numofBuffer, int sizeOfImage)
		{
            // Allocate Unmanaged memory area..
            buffer = new byte[numofBuffer][];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = new byte[sizeOfImage];
            }
		}

		public int COLORBYTES = 3;
        public int MONOBYTES = 1;
        public const int EDOFRECSIZE = 5;
		public const int XYZNUMS = 3;

		public AcquisitionFixedParameters afp;

		public int PixelSize() { return afp.cameraRoiWidth * afp.cameraRoiHeight; }
		public int ImgSize() { return PixelSize() * COLORBYTES; }
		public int DataSize() { return PixelSize() * sizeof(float); }
		public int EdofSize() { return EDOFRECSIZE; }
		public bool IsColor() { return (COLORBYTES == 3); }
		public int PixelWidth() { return afp.cameraRoiWidth; }
		public int PixelHeight() { return afp.cameraRoiHeight; }

		public tRingBuf rb;
		public cFocus focus = new cFocus();
        
        public byte[][] buffer;
    }

    public class ViewerData
	{
		public IntPtr vHwnd;
		public PictureBox pBox;

		public ViewerData(IntPtr handle, PictureBox box)
		{
			vHwnd = handle;
			pBox = box;
		}
	}
}
