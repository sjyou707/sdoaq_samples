using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SDOAQ_App_CS
{
    public class RingBuffer : IDisposable
    {
        public int NumOfBuffer => Buffer?.Length ?? 0;
        public IntPtr[] Buffer;
        public ulong[] Sizes;

        public void Clear()
        {
            if (Buffer == null)
            {
                return;
            }

            foreach (var buff in Buffer)
            {
                if (buff != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buff);
                }
            }

            Buffer = null;
            Sizes = null;
        }

        public void Set_Buffer(ulong[] sizes)
        {
            if (sizes == null)
            {
                return;
            }

            Clear();

            Buffer = new IntPtr[sizes.Length];
            Sizes = sizes;
            for (int i = 0; i < sizes.Length; i++)
            {
                int size = (int)sizes[i];

                if (size > 0)
                {
                    Buffer[i]  = Marshal.AllocHGlobal(size);
                }
                else
                {
                    Buffer[i] = IntPtr.Zero;
                }
            }
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
