using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace C0D3RMem.Memory
{
	public class Reader
	{
		private IntPtr pHandler = IntPtr.Zero;
		public Reader(IntPtr pHandle)
		{
			this.pHandler = pHandle;
		}

		public long GetAddrAfterOffset(long addr, long[] offsets)
		{
            long ptr = IntPtr.Zero;
			for (int i = 0; i < offsets.Length; i++)
			{
				ptr = ReadInt64(addr + offsets[i]);
			}
			return ptr;
		}

		public byte[] ReadBytes(long addr, int length)
		{
			byte[] bytes = new byte[length];
			API.ReadProcessMemory(this.pHandler, addr, bytes, (ulong)bytes.Length, out nint ByteRead);
			return bytes;
		}
		public int ReadInt32(long addr)
		{
			byte[] buffer = new byte[4];
			API.ReadProcessMemory(this.pHandler, addr, buffer, (ulong)buffer.Length, out nint ByteRead);
			return BitConverter.ToInt32(buffer, 0);
		}
		public uint ReadUInt32(long addr)
		{
			byte[] buffer = new byte[4];
			API.ReadProcessMemory(this.pHandler, addr, buffer, (ulong)buffer.Length, out nint ByteRead);
			return BitConverter.ToUInt32(buffer, 0);
		}
		public long ReadInt64(long addr)
		{
			byte[] buffer = new byte[8];
			API.ReadProcessMemory(this.pHandler, addr, buffer, (ulong)buffer.Length, out nint ByteRead);
			return BitConverter.ToInt64(buffer, 0);
		}
		public ulong ReadUInt64(long addr)
		{
			byte[] buffer = new byte[8];
			API.ReadProcessMemory(this.pHandler, addr, buffer, (ulong)buffer.Length, out nint ByteRead);
			return BitConverter.ToUInt64(buffer, 0);
		}
		public float ReadFloat(long addr)
		{
			byte[] buffer = new byte[sizeof(float)];
			API.ReadProcessMemory(this.pHandler, addr, buffer, (ulong)buffer.Length, out nint ByteRead);
			return BitConverter.ToSingle(buffer, 0);
		}
		public string ReadString(long addr, ulong _size)
		{
			byte[] buffer = new byte[_size];
			API.ReadProcessMemory(this.pHandler, addr, buffer, _size, out nint ByteRead);
			return Encoding.UTF8.GetString(buffer);
		}

		// Read Vector3
		public Vector3 ReadVector3(long addr)
		{
			Vector3 tmp = new Vector3();
			byte[] Buffer = new byte[12];

			API.ReadProcessMemory(this.pHandler, addr, Buffer, 12, out nint ByteRead);
			tmp.X = BitConverter.ToSingle(Buffer, (0 * 4));
			tmp.Y = BitConverter.ToSingle(Buffer, (1 * 4));
			tmp.Z = BitConverter.ToSingle(Buffer, (2 * 4));

			return tmp;
		}
		// Read Matrix
		public Matrix4x4 ReadMatrix(long addr)
		{
			Matrix4x4 tmp = new Matrix4x4();

			byte[] Buffer = new byte[64];

			API.ReadProcessMemory(this.pHandler, addr, Buffer, 64, out nint ByteRead);

			tmp.M11 = BitConverter.ToSingle(Buffer, (0 * 4));
			tmp.M12 = BitConverter.ToSingle(Buffer, (1 * 4));
			tmp.M13 = BitConverter.ToSingle(Buffer, (2 * 4));
			tmp.M14 = BitConverter.ToSingle(Buffer, (3 * 4));

			tmp.M21 = BitConverter.ToSingle(Buffer, (4 * 4));
			tmp.M22 = BitConverter.ToSingle(Buffer, (5 * 4));
			tmp.M23 = BitConverter.ToSingle(Buffer, (6 * 4));
			tmp.M24 = BitConverter.ToSingle(Buffer, (7 * 4));

			tmp.M31 = BitConverter.ToSingle(Buffer, (8 * 4));
			tmp.M32 = BitConverter.ToSingle(Buffer, (9 * 4));
			tmp.M33 = BitConverter.ToSingle(Buffer, (10 * 4));
			tmp.M34 = BitConverter.ToSingle(Buffer, (11 * 4));

			tmp.M41 = BitConverter.ToSingle(Buffer, (12 * 4));
			tmp.M42 = BitConverter.ToSingle(Buffer, (13 * 4));
			tmp.M43 = BitConverter.ToSingle(Buffer, (14 * 4));
			tmp.M44 = BitConverter.ToSingle(Buffer, (15 * 4));

			return tmp;
		}
	}
}
