using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace C0D3RMem.Memory
{
	public class Scanner
	{
		private Process tProcess;
		private IntPtr pHandler;

		public Scanner(Process proc)
		{
			tProcess = proc;
			pHandler = API.OpenProcess(API.PROCESS_ALL_ACCESS, false, tProcess.Id);
		}

		/// <summary>
		/// Scans memory for a given byte signature with `?`
		/// </summary>
		/// <param name="byteSignature"></param>
		/// <returns></returns>
		public List<IntPtr> ScanMemory(byte[] byteSignature)
		{
			List<IntPtr> result = new();

			IntPtr currentAddr = IntPtr.Zero;
			nint bytesRead = 0;

			while (API.VirtualQueryEx(pHandler, currentAddr, out API.MEMORY_BASIC_INFORMATION mbi, (uint)Marshal.SizeOf(typeof(API.MEMORY_BASIC_INFORMATION))))
			{
				if (mbi.State == API.MEM_COMMIT && (mbi.Protect != API.PAGE_READWRITE || mbi.Protect != API.PAGE_READONLY))
				{
					byte[] buffer = new byte[(int)mbi.RegionSize];
					if (API.ReadProcessMemory(pHandler, mbi.BaseAddress, buffer, (ulong)buffer.Length, out bytesRead))
					{
						for (int i = 0; i < bytesRead - byteSignature.Length; i++)
						{
							bool match = true;
							for (int j = 0; j < byteSignature.Length; j++)
							{
								if (byteSignature[j] != 0x0 && buffer[i + j] != byteSignature[j])
								{
									match = false;
									break;
								}
							}
							if (match)
							{
								result.Add(mbi.BaseAddress + i);
							}
						}
					}
				}
				currentAddr = new(currentAddr.ToInt64() + mbi.RegionSize.ToInt64());
			}
			return result;
		}
	}
}
