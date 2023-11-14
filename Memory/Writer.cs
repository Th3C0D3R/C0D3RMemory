using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C0D3RMem.Memory
{


    public class Writer
    {
        private IntPtr pHandler = IntPtr.Zero;
        private Reader reader;

        public Writer(IntPtr pHandle, Reader read)
        {
            this.pHandler = pHandle;
            this.reader = read;
        }

        public nint WriteMemory(Int64 addr, byte[] Buffer)
        {
            API.VirtualProtectEx(pHandler, (IntPtr)addr, (uint)Buffer.Length, API.PAGE_READWRITE, out uint oldProtect);

            API.WriteProcessMemory(pHandler, addr, Buffer, (uint)Buffer.Length, out nint ptrBytesWritten);

            return ptrBytesWritten;
        }
        public nint Write(Int64 baseAddr, dynamic _value, long[]? offsets = null)
        {
            byte[] buffer = BitConverter.GetBytes(_value);

            if (offsets != null)
            {
                baseAddr = reader.GetAddrAfterOffset(baseAddr, offsets);
            }
            return WriteMemory(baseAddr, buffer);
        }
    }
}
